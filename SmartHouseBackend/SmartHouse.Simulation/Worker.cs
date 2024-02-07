using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using System.Text.Json;

namespace SmartHouse.Simulation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MqttClientOptions _mqttOptions;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger,
            MqttClientOptions mqttOptions,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _mqttOptions = mqttOptions;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            await client.ConnectAsync(_mqttOptions, CancellationToken.None);
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<SmartDevice>>();

            var devices = await repository.FindAll();
            await SetDeviceStatusToOnline(repository, devices);

            while (!stoppingToken.IsCancellationRequested)
            {
                devices = await repository.FindByCondition(device => device.IsOnline == true);

                foreach (var device in devices)
                {
                    Random random = new Random();

                    double randomNumber = random.NextDouble();

                    if (randomNumber > 0.5)
                    {
                        await SendOnlineStatus(client, device.Id, device.Name);

                    }

                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private static async Task SetDeviceStatusToOnline(IRepositoryBase<SmartDevice> repository, ICollection<SmartDevice> devices)
        {
            foreach (var device in devices)
            {
                device.IsOnline = true;
            }
            await repository.SaveChanges();
        }

        private static async Task SendOnlineStatus(IMqttClient client, Guid id, string name)
        {
            string topic = "online";
            var payload = new SendPingDTO()
            {
                Id = id,
                Name = name,
            };
            string payloadJson = JsonSerializer.Serialize(payload);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payloadJson)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await client.PublishAsync(message, CancellationToken.None);
        }
    }
}