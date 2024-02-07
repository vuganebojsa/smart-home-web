using Microsoft.EntityFrameworkCore;
using MQTTnet.Client;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Repositories;
using SmartHouse.Simulation;

var mqttOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .Build();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton(mqttOptions);
        // Retrieve the connection string from the configuration
        IConfiguration configuration = hostContext.Configuration;
        string connectionString = "Server=VUGA;Database=SmartHome;Trusted_Connection=True;TrustServerCertificate=True";

        services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IRepositoryBase<SmartDevice>, SmartDeviceRepository>();


    })
    .Build();

await host.RunAsync();
