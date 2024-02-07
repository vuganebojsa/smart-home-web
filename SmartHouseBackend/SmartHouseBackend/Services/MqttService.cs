using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Hubs;
using SmartHouse.Hubs.interfaces;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Interfaces.Services;
using SmartHouse.Infrastructure.MqttDTOs;
using System.Text;
using System.Text.Json;

namespace SmartHouse.Services
{
    public class MqttService : BackgroundService, IMqttService
    {
        //private readonly IMqttClientOptions _mqttOptions;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<GateEvent, IDeviceStatus> _gateEventHubs;
        private readonly IHubContext<SensorData, IDeviceStatus> _sensorDataHubs;
        private readonly IHubContext<DeviceStatus, IDeviceStatus> _deviceStatusHubs;
        private readonly IHubContext<SolarPanelSystemProducedPower, IDeviceStatus> _spsPowerHubs;
        private readonly IHubContext<VehicleChargingStatus, IDeviceStatus> _vehicleChargeHubs;
        private IMqttClient _client;
        private readonly IServiceScopeFactory _scopeFactory;





        public MqttService(IConfiguration configuration, IServiceProvider serviceProvider,
            IHubContext<DeviceStatus, IDeviceStatus> deviceStatusHubs, IHubContext<GateEvent, IDeviceStatus> gateEventHubs, IHubContext<SolarPanelSystemProducedPower, IDeviceStatus> spsPowerHubs, IServiceScopeFactory scopeFactory
            , IHubContext<SensorData, IDeviceStatus> sensorDataHubs,
            IHubContext<VehicleChargingStatus, IDeviceStatus> vehicleChargeHubs,
            IMqttClient client)

        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _deviceStatusHubs = deviceStatusHubs;

            _gateEventHubs = gateEventHubs;
            _sensorDataHubs = sensorDataHubs;
            _vehicleChargeHubs = vehicleChargeHubs;
            _spsPowerHubs = spsPowerHubs;
            _scopeFactory = scopeFactory;
            _client = client;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();




            await RegisterToTopics(_client);

            _client.UseApplicationMessageReceivedHandler(e => HandleMqttMessage(e));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);
                await CheckLastPing(_client);
            }
        }

        private async Task<IMqttClient> CreateClient(IMqttClientOptions mqttOptions)
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            await client.ConnectAsync(mqttOptions, CancellationToken.None);
            return client;
        }

        private async Task RegisterToTopics(IMqttClient client)
        {
            var onlineTopic = _configuration["Mqtt:OnlineTopic"];
            var deviceOnOffTopic = _configuration["Mqtt:DeviceOnOff"];
            var solarPanelTopic = _configuration["Mqtt:SolarPanelTopic"];
            var batteryOccupationTopic = _configuration["Mqtt:BatteryOccupationTopic"];
            var lampLuminostiyTopic = _configuration["Mqtt:LampLuminosityTopic"];
            var gateEventTopic = _configuration["Mqtt:GateEventTopic"];
            var gatePublicPrivateTopic = _configuration["Mqtt:TurnGatePublicPrivate"];
            var ambientSensorRoomDataTopic = _configuration["Mqtt:AmbientSensorRoomDataTopic"];
            var changeSpecialStateSprinkler = _configuration["Mqtt:SprinklerSpecialStateChanged"];
            var sprinklerOnOff = _configuration["Mqtt:SprinklerOnOff"];


            var vehicleChargerPowerUsedTopic = _configuration["Mqtt:VehicleChargerPowerUsed"];
            var vehicleChargerPowerStartCharge = _configuration["Mqtt:VehicleChargerStartCharge"];
            await client.SubscribeAsync(gatePublicPrivateTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(onlineTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(deviceOnOffTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(solarPanelTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(batteryOccupationTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(lampLuminostiyTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(gateEventTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(ambientSensorRoomDataTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(changeSpecialStateSprinkler, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(sprinklerOnOff, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);


            await client.SubscribeAsync(vehicleChargerPowerUsedTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            await client.SubscribeAsync(vehicleChargerPowerStartCharge, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);

        }

        private async Task CheckLastPing(IMqttClient client)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();

            DateTime timeBefore30Seconds = DateTime.Now.AddSeconds(-30);
            TimeSpan threshold = TimeSpan.FromSeconds(30);

            var devices = await repository.FindByConditionIncludesProperty(device => timeBefore30Seconds.CompareTo(device.LastPingTime) > 0 && device.IsOnline);

            foreach (var device in devices)
            {
                device.IsOnline = false;
                var deviceStatus = new DeviceStatusDTO()
                {
                    Id = device.Id,
                    IsOnline = device.IsOnline,
                };
                await _deviceStatusHubs.Clients.Group(device.SmartProperty.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(deviceStatus));
                var offlineTopic = $"offline/{device.Id}";
                await PublishOfflineTopic(offlineTopic, device.Id, client);
            }
            if (devices.Count > 0)
            {
                await repository.SaveChanges();
            }
        }
        private async Task PublishOfflineTopic(string offlineTopic, Guid id, IMqttClient client)
        {
            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(offlineTopic)
                .WithPayload("")
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();
            influx.Write(write =>
            {
                var point = PointData.Measurement("onlineStatus")
                    .Tag("deviceId", id.ToString())
                    .Field("isOnline", 0)
                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                write.WritePoint(point, "smarthome", "FTN");
            });

            await client.PublishAsync(message);
        }

        private async void HandleMqttMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            string payloadJson = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            if (topic == _configuration["Mqtt:OnlineTopic"])
            {
                HandleOnlineTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:DeviceOnOff"])
            {
                await HandleDeviceOnOffTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:SolarPanelTopic"])
            {
                await HandleSolarPanelSystemTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:BatteryOccupationTopic"])
            {
                await HandleBatteryStatusTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:LampLuminosityTopic"])
            {

                await HandleLampLuminosityTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:GateEventTopic"])
            {

                await HandleGateEventTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:TurnGatePublicPrivate"])
            {
                await HandleGatePublicPrivateTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:AmbientSensorRoomDataTopic"])
            {
                await HandleSensorRoomDataTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:SprinklerSpecialStateChanged"])
            {
                await HandleSprinklerSpecialStateTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:SprinklerOnOff"])
            {
                await HandleSprinklerOnOffTopic(payloadJson);
            }
            else if (topic == _configuration["Mqtt:VehicleChargerPowerUsed"])
            {
                await HandleVehicleChargerPowerUsed(payloadJson);
            }
            else if (topic == _configuration["Mqtt:VehicleChargerStartCharge"])
            {
                await HandleVehicleChargerStartCharge(payloadJson);
            }
        }

        private async Task HandleSprinklerOnOffTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var message = JsonSerializer.Deserialize<OnOffDTO>(payloadJson);
            influx.Write(write =>
            {
                var point = PointData.Measurement("sprinkleEvent")
                                .Tag("deviceId", message.deviceId.ToString())
                                .Tag("username", "System")
                                .Tag("actionNumber", "1")
                                .Field("value", message.on.ToString())

                                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                write.WritePoint(point, "smarthome", "FTN");
            });

        }

        private async Task HandleVehicleChargerStartCharge(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var message = JsonSerializer.Deserialize<VehicleStartChargeDTO>(payloadJson);
            var chrgr = await repository.FindById(Guid.Parse(message.DeviceId));
            if (chrgr == null) { return; }
            influx.Write(write =>
            {
                var point = PointData.Measurement("vehiclecharging")
                    .Tag("deviceId", chrgr.Id.ToString())
                    .Tag("plate", message.Plate)
                    .Tag("minutesNeeded", message.MinutesNeeded.ToString() + " min")
                    .Field("startChargeNeededKW", message.ToCharge)
                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                write.WritePoint(point, "smarthome", "FTN");
            });
            message.Status = "Start";
            await _vehicleChargeHubs.Clients.Group(chrgr.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(message));
        }

        private async Task HandleVehicleChargerPowerUsed(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var message = JsonSerializer.Deserialize<VehicleEndChargeDTO>(payloadJson);

            var chrgr = await repository.FindById(Guid.Parse(message.DeviceId));
            if (chrgr == null) { return; }
            var property = await repository.GetPropertyByDeviceId(chrgr.Id);
            if (property == null) { return; }
            if (chrgr is ElectricVehicleCharger charger)
            {
                influx.Write(write =>
                {
                    var point = PointData.Measurement("vehiclecharging")
                        .Tag("deviceId", chrgr.Id.ToString())
                        .Tag("plate", message.Plate)
                        .Tag("minutesNeeded", message.MinutesNeeded.ToString() + " min")
                        .Field("endChargeConsumedKWH", message.TotalKwhConsumed)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
                var kwhConsumed = message.TotalKwhConsumed;

                var batteries = await repository.GetBatteriesByProperty(property.Id);
                var leftover = kwhConsumed;

                foreach (var battery in batteries)
                {
                    if (battery.OccupationLevel >= leftover)
                    {
                        battery.OccupationLevel -= leftover;
                        leftover = 0;
                    }
                    else
                    {
                        leftover = leftover - battery.OccupationLevel;
                        battery.OccupationLevel = 0;
                    }

                    await PublishToBatteryOccupation(battery);

                }
                await repository.SaveChanges();
                if (leftover > 0)
                {
                    influx.Write(write =>
                    {

                        var point = PointData.Measurement("electricenergy")
                              .Tag("smartPropertyId", property.Id.ToString())
                              .Tag("city", property.City)
                        .Field("takenFromElectricDistribution", leftover)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                        write.WritePoint(point, "smarthome", "FTN");
                    });


                }

                influx.Write(write =>
                {

                    var point = PointData.Measurement("energyconsumed")
                          .Tag("smartPropertyId", property.Id.ToString())
                          .Tag("city", property.City)
                    .Field("totalConsumedByProperty", kwhConsumed)
                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
                await _vehicleChargeHubs.Clients.Group(chrgr.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(message));
            }


        }

        private async Task HandleBatteryStatusTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var message = JsonSerializer.Deserialize<BatteryOccupationDTO>(payloadJson);
            var battery = await repository.FindById(Guid.Parse(message.DeviceId));

            if (battery == null) { return; }
            if (battery is HouseBattery btr)
            {

                await _spsPowerHubs.Clients.Group(battery.Id.ToString()).ReceiveMessage(message.BatteryOccupation.ToString());
                btr.OccupationLevel = message.BatteryOccupation;
                await repository.SaveChanges();
            }

        }

        private async Task HandleSolarPanelSystemTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var message = JsonSerializer.Deserialize<SolarPanelSystemReceiveKwhDTO>(payloadJson);
            var device = await repository.FindById(Guid.Parse(message.DeviceId));
            if (device == null) { return; }

            var property = await repository.GetPropertyByDeviceId(device.Id);
            if (device.IsOnline)
            {
                influx.Write(write =>
                {
                    var point = PointData.Measurement("electricenergy")
                        .Tag("deviceId", device.Id.ToString())
                        .Tag("smartPropertyId", property.Id.ToString())
                        .Tag("city", property.City)
                        .Field("totalProduced", message.TotalKwhPerMinute)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });



                await _spsPowerHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(message.TotalKwhPerMinute));
                var smartProperty = await repository.GetPropertyByDeviceId(device.Id);
                if (smartProperty == null)
                {
                    return;
                }
                if (message.TotalKwhPerMinute == 0) { return; }

                await AddPowerToBatteries(device, message.TotalKwhPerMinute, smartProperty);

            }

        }

        private async Task HandleSensorRoomDataTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();

            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();

            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();

            var message = JsonSerializer.Deserialize<AmbientSensorDataDTO>(payloadJson);
            var device = await repository.FindById(Guid.Parse(message.DeviceId));

            if (device == null) { return; }
            if (device.IsOnline)
            {

                influx.Write(write =>
                {


                    var point = PointData.Measurement("humidity")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("roomHumidity", message.RoomHumidity)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);
                    write.WritePoint(point, "smarthome", "FTN");


                    point = PointData.Measurement("temperature")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("roomTemperature", message.RoomTemperature)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);
                    write.WritePoint(point, "smarthome", "FTN");


                });
                await _sensorDataHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
                {
                    timeStamp = DateTime.UtcNow,
                    temperature = message.RoomTemperature,
                    humidity = message.RoomHumidity
                }));
            }

        }

        private async Task HandleLampLuminosityTopic(string payloadJson)
        {

            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var message = JsonSerializer.Deserialize<LampLuminosityDTO>(payloadJson);
            var device = await repository.FindById(Guid.Parse(message.DeviceId));

            if (device == null) { return; }
            if (device.IsOnline)
            {

                influx.Write(write =>
                {


                    var point = PointData.Measurement("luminosity")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("detected", message.TotalLuminosity)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);


                    write.WritePoint(point, "smarthome", "FTN");
                });
            }

        }

        private async Task HandleGateEventTopic(string payloadJson)
        {

            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var message = JsonSerializer.Deserialize<GateEventDTO>(payloadJson);
            var device = await repository.FindById(Guid.Parse(message.DeviceId));


            if (device == null) { return; }
            if (device.IsOnline)
            {

                influx.Write(write =>
                {
                    var point = PointData.Measurement("event")
                        .Tag("deviceId", device.Id.ToString())
                        .Tag("licencePlate", message.licencePlate)
                        .Field("action", message.action)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
                await _gateEventHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
                {
                    licencePlate = message.licencePlate,
                    action = message.action,
                    timeStamp = DateTime.UtcNow
                }));

            }

        }


        private async Task AddPowerToBatteries(SmartDevice device, double totalKwhPerMinute, SmartProperty smartProperty)

        {
            using var scope = _serviceProvider.CreateScope();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var influxPoints = new List<PointData>();

            var user = await repository.GetUserByDeviceId(device.Id);
            if (user == null)
            {
                return;
            }
            var batteries = await repository.GetBatteriesByProperty(smartProperty.Id);
            if (batteries == null) { return; }
            if (batteries.Count == 0) { return; }
            double kwhToAddPerBattery = totalKwhPerMinute / batteries.Count;
            double leftover = 0;

            foreach (var battery in batteries)
            {

                if (battery.OccupationLevel <= battery.BatterySize - kwhToAddPerBattery)
                {
                    battery.OccupationLevel += kwhToAddPerBattery;
                    double leftoverToAdd = leftover - (battery.BatterySize - battery.OccupationLevel);

                    if (leftoverToAdd > 0 && battery.OccupationLevel <= battery.BatterySize - leftoverToAdd)
                    {
                        battery.OccupationLevel += leftoverToAdd;
                        leftover -= leftoverToAdd;
                    }

                }
                else
                {
                    var totalKwhLeft = kwhToAddPerBattery - (battery.BatterySize - battery.OccupationLevel);
                    battery.OccupationLevel = battery.BatterySize;
                    if (totalKwhLeft > 0)
                    {
                        leftover += totalKwhLeft;
                    }
                }
                var point = PointData.Measurement("batteryoccupation")
                .Tag("deviceId", battery.Id.ToString())
                .Field("currectOccupationLevel", battery.OccupationLevel)
                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);
                influxPoints.Add(point);

                await PublishToBatteryOccupation(battery);

            }
            await repository.SaveChanges();
            if (leftover > 0)
            {

                var point = PointData.Measurement("electricenergy")
                    .Tag("smartPropertyId", smartProperty.Id.ToString())
                    .Field("sendToElectricDistribution", leftover)
                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);
                influxPoints.Add(point);
            }
            influx.Write(write => write.WritePoints(influxPoints, "smarthome", "FTN"));


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

        private async Task HandleDeviceOnOffTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var repo = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var message = JsonSerializer.Deserialize<OnOffDTO>(payloadJson);

            var device = await repository.FindById(Guid.Parse(message.deviceId));
            var property = await repo.GetPropertyByDeviceId(Guid.Parse(message.deviceId));
            var prevStatus = device.IsOn;
            if (device != null)
            {
                if (device.IsOn && message.on)
                {
                    return;
                }
                if (!device.IsOn && !message.on) { return; }

                device.IsOn = message.on;
                await repository.SaveChanges();

                var deviceStatus = new OnOffDTO()
                {
                    deviceId = device.Id.ToString(),
                    on = device.IsOn,
                };

                var isOn = 0;
                if (device.IsOn)
                {
                    isOn = 1;
                }
                influx.Write(write =>
                {
                    var point = PointData.Measurement("status")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("isOn", isOn)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });

                if (prevStatus != device.IsOn)
                {
                    await _deviceStatusHubs.Clients.Group(property.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(deviceStatus));
                    await _gateEventHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
                    {

                        onOff = deviceStatus.on,
                        timeStamp = DateTime.UtcNow
                    }));

                }

            }
        }

        private async Task HandleGatePublicPrivateTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var repo = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var message = JsonSerializer.Deserialize<PublicPrivateDTO>(payloadJson);

            var device = await repo.GetVehicleGateById(Guid.Parse(message.deviceId));
            var property = await repo.GetPropertyByDeviceId(Guid.Parse(message.deviceId));

            if (device != null)
            {
                var prevStatus = device.IsPublic;
                if (device.IsPublic && message.isPublic)
                {
                    return;
                }
                if (!device.IsPublic && !message.isPublic) { return; }

                device.IsPublic = message.isPublic;
                await repo.SaveChanges();

                var gateStatus = new PublicPrivateDTO()
                {
                    deviceId = device.Id.ToString(),
                    isPublic = device.IsPublic,
                };


                var isPublic = 0;
                if (device.IsPublic)
                {
                    isPublic = 1;
                }

                influx.Write(write =>
                {
                    var point = PointData.Measurement("gateStatus")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("isPublic", isPublic)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
                if (prevStatus != device.IsPublic)
                {
                    await _gateEventHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
                    {

                        publicPrivate = gateStatus.isPublic,
                        timeStamp = DateTime.UtcNow
                    }));

                }

            }
        }

        private async Task HandleSprinklerSpecialStateTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();
            var repo = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();
            var message = JsonSerializer.Deserialize<PublicPrivateDTO>(payloadJson);

            var device = await repo.GetSprinklerById(Guid.Parse(message.deviceId));
            var property = await repo.GetPropertyByDeviceId(Guid.Parse(message.deviceId));
            if (device != null)
            {
                var prevStatus = device.IsSpecialMode;
                if (device.IsSpecialMode && message.isPublic)
                {
                    return;
                }
                if (!device.IsSpecialMode && !message.isPublic) { return; }

                device.IsSpecialMode = message.isPublic;
                await repo.SaveChanges();

                var gateStatus = new PublicPrivateDTO()
                {
                    deviceId = device.Id.ToString(),
                    isPublic = device.IsSpecialMode,
                };


                var isPublic = 0;
                if (device.IsSpecialMode)
                {
                    isPublic = 1;
                }

                influx.Write(write =>
                {
                    var point = PointData.Measurement("sprinklerStatus")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("isPublic", isPublic)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });


            }
        }

        private async void HandleOnlineTopic(string payloadJson)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISmartDeviceRepository>();
            var influx = scope.ServiceProvider.GetRequiredService<IInfluxDbService>();


            SendPingDTO message = JsonSerializer.Deserialize<SendPingDTO>(payloadJson);
            var id = message.Id;

            var device = await repository.FindByIdWithProperty(sd => sd.Id == id);
            if (device == null)
            {
                return;
            }
            var prevStatus = device.IsOnline;
            device.IsOnline = true;
            device.LastPingTime = DateTime.Now;
            await repository.SaveChanges();
            var deviceStatus = new DeviceStatusDTO()
            {
                Id = device.Id,
                IsOnline = device.IsOnline,
            };
            var isOnline = 0;
            if (device.IsOnline)
            {
                isOnline = 1;
            }

            if (prevStatus != device.IsOnline)
            {
                influx.Write(write =>
                {
                    var point = PointData.Measurement("onlineStatus")
                        .Tag("deviceId", device.Id.ToString())
                        .Field("isOnline", isOnline)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });

                await _deviceStatusHubs.Clients.Group(device.SmartProperty.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(deviceStatus));
                await _gateEventHubs.Clients.Group(device.Id.ToString()).ReceiveMessage(JsonSerializer.Serialize(new
                {

                    online = deviceStatus.IsOnline,
                    timeStamp = DateTime.UtcNow
                }));

            }
        }
    }
}
