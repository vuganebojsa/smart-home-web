using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Hubs;
using SmartHouse.Hubs.interfaces;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Interfaces.Services;
using System.Text.Json;

namespace SmartHouse.Services
{
    public class PowerService : BackgroundService, IPowerService
    {
        // private readonly IMqttClientOptions _mqttOptions;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<EnergyRequiredByProperty, IDeviceStatus> _energyStatusHubs;
        private IMqttClient _client;
        private readonly IServiceScopeFactory _scopeFactory;


        public PowerService(IConfiguration configuration, IServiceProvider serviceProvider,
            IHubContext<EnergyRequiredByProperty, IDeviceStatus> deviceStatusHubs, IServiceScopeFactory scopeFactory, IMqttClient client)
        {
            _configuration = configuration;
            // _mqttOptions = mqttOptions;
            _serviceProvider = serviceProvider;
            _energyStatusHubs = deviceStatusHubs;
            _scopeFactory = scopeFactory;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();



            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(60000, stoppingToken);
                await DistributeElectricity();
            }
        }

        private async Task<IMqttClient> CreateClient(IMqttClientOptions mqttOptions)
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            await client.ConnectAsync(mqttOptions, CancellationToken.None);
            return client;
        }
        private async Task DistributeElectricity()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var smartPropertyRepository = scope.ServiceProvider.GetRequiredService<ISmartPropertyRepository>();
            var smartProperties = await smartPropertyRepository.FindTotal(200);

            foreach (var smartProperty in smartProperties)
            {
                await DistributePerProperty(scope, repository, smartProperty);
            }

        }

        private async Task DistributePerProperty(IServiceScope scope, ISmartDeviceRepository repository, SmartProperty smartProperty)
        {
            double totalEnergyInBateries = await repository.GetTotalBateryCapacityByPropertyId(smartProperty.Id);
            double totalEnergyConsumption = await repository.GetTotalEnergyConsumptionByPropertyId(smartProperty.Id);
            totalEnergyConsumption = totalEnergyConsumption / 60;
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();

            influx.Write(write =>
            {

                var point = PointData.Measurement("energyconsumed")
                      .Tag("smartPropertyId", smartProperty.Id.ToString())
                      .Tag("city", smartProperty.City)
                .Field("totalConsumedByProperty", totalEnergyConsumption)
                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                write.WritePoint(point, "smarthome", "FTN");
            });
            await _energyStatusHubs.Clients.Group(smartProperty.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
            {
                energyRequired = totalEnergyConsumption
            }));
            var difference = totalEnergyInBateries - totalEnergyConsumption;

            var batteries = await repository.GetBatteriesByProperty(smartProperty.Id);
            if (difference == 0)
            {
                foreach (var battery in batteries)
                {
                    battery.OccupationLevel = 0;
                    await PublishToBatteryOccupation(battery);

                }
                await repository.SaveChanges();
            }
            else if (difference > 0)
            {
                foreach (var battery in batteries)
                {
                    if (battery.OccupationLevel >= difference)
                    {
                        battery.OccupationLevel -= difference;
                        break;
                    }
                    else
                    {
                        var leftover = difference - battery.OccupationLevel;
                        battery.OccupationLevel = 0;
                        difference = leftover;
                    }
                    await repository.SaveChanges();
                    await PublishToBatteryOccupation(battery);
                }

            }
            else if (difference < 0)
            {

                foreach (var battery in batteries)
                {
                    battery.OccupationLevel = 0;
                    await PublishToBatteryOccupation(battery);

                }
                await repository.SaveChanges();

                influx.Write(write =>
                {

                    var point = PointData.Measurement("electricenergy")
                          .Tag("smartPropertyId", smartProperty.ToString())
                    .Field("takenFromElectricDistribution", Math.Abs(difference))
                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
            }
        }

        private async Task PublishToBatteryOccupation(HouseBattery battery)
        {
            var message = new MqttApplicationMessageBuilder()
                                .WithTopic("occupation/" + battery.Id.ToString())
                                .WithPayload(battery.OccupationLevel.ToString())
                                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                                .WithRetainFlag(false)
                                .Build();

            await _client.PublishAsync(message);
        }

    }
}
