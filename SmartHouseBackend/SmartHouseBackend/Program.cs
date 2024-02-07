using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SmartHouse;
using SmartHouse.Core.Interfaces.Services;
using SmartHouse.Core.Model;
using SmartHouse.Filters;
using SmartHouse.Hubs;
using SmartHouse.Infrastructure;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Interfaces.Services;
using SmartHouse.Infrastructure.Repositories;
using SmartHouse.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<Seed>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISmartDeviceService, SmartDeviceService>();
builder.Services.AddScoped<IRepositoryBase<User>, UserRepository>();
builder.Services.AddScoped<IRepositoryBase<SmartDevice>, SmartDeviceRepository>();
builder.Services.AddScoped<IRepositoryBase<Cycle>, CycleRepository>();
builder.Services.AddScoped<IRepositoryBase<Panel>, PanelRepository>();
builder.Services.AddScoped<ISmartPropertyService, SmartPropertyService>();
builder.Services.AddScoped<IRepositoryBase<SmartProperty>, SmartPropertyRepository>();
builder.Services.AddScoped<IRepositoryBase<City>, CityRepository>();
builder.Services.AddScoped<IRepositoryBase<Country>, CountryRepository>();
builder.Services.AddScoped<ISmartPropertyRepository, SmartPropertyRepository>();
builder.Services.AddScoped<ISmartDeviceRepository, SmartDeviceRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMqttService, MqttService>();
builder.Services.AddScoped<IPowerService, PowerService>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IInfluxDbService, InfluxDBService>();


builder.Services.AddSingleton<IMqttClient>(provider =>
{
    var mqttOptions = new MqttClientOptionsBuilder()
                   .WithTcpServer("192.168.105.29", 1883)
                   .WithKeepAlivePeriod(TimeSpan.FromMinutes(5))
                   .Build();

    var factory = new MqttFactory();
    var mqttClient = factory.CreateMqttClient();
    mqttClient.ConnectAsync(mqttOptions).GetAwaiter().GetResult();

    return mqttClient;
});

builder.Services.AddHostedService<MqttService>();
builder.Services.AddHostedService<PowerService>();
builder.Services.AddDbContext<DataContext>(

    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("."), b => b.MigrationsAssembly("SmartHouse.Infrastructure"));
    }, ServiceLifetime.Scoped
);

builder.Services.AddControllers(
     options =>
     { options.Filters.Add(typeof(ModelStateValidationFilter)); }

);

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("bivuja")))
        };
    }
    );

builder.Services.AddSignalR();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

    // Configure OAuth2 for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {tokeninfo}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowOrigins", policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowCredentials().AllowAnyHeader();
        });
    }
    );
var app = builder.Build();


SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetService<Seed>();
    service.SeedDataContext();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowOrigins");

app.UseAuthentication();
app.UseAuthorization();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

    dbContext.Database.Migrate();

}

app.MapControllers();

app.MapHub<DeviceStatus>("/hubs/status");

app.MapHub<GateEvent>("/hubs/gateEvent");

app.MapHub<SensorData>("/hubs/sensorData");

app.MapHub<SolarPanelSystemProducedPower>("/hubs/spspower");
app.MapHub<EnergyRequiredByProperty>("/hubs/energy");
app.MapHub<VehicleChargingStatus>("/hubs/vehiclecharge");


app.Run();
