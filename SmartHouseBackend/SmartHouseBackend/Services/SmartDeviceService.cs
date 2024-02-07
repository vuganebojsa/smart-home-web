using AutoMapper;
using FluentResults;
using InfluxDB.Client.Writes;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using SmartHouse.Core.Messages;
using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Core.Model.OutsideSmartDevices;
using SmartHouse.Core.Model.SmartHomeDevices;
using SmartHouse.Extensions;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Interfaces.Services;
using File = System.IO.File;
using User = SmartHouse.Core.Model.User;

namespace SmartHouse.Services
{
    public class SmartDeviceService : ISmartDeviceService
    {
        private readonly ISmartDeviceRepository _smartDeviceRepository;
        private readonly ISmartPropertyRepository _smartPropertyRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IRepositoryBase<Cycle> _cycleRepository;
        private readonly IRepositoryBase<Panel> _panelRepository;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;


        private IMqttClient _mqttClient;
        private IInfluxDbService _influxService;
        public SmartDeviceService(ISmartDeviceRepository smartDeviceRepository,
            IServiceProvider serviceProvider,
            ISmartPropertyRepository smartPropertyRepository,
            IRepositoryBase<User> userRepository,
            IRepositoryBase<Cycle> cycleRepository,
            IRepositoryBase<Panel> panelRepository,
            IConfiguration configuration,
            IInfluxDbService influxService,
            IMapper mapper,
            IMqttClient client)
        {

            _serviceProvider = serviceProvider;

            _smartDeviceRepository = smartDeviceRepository;
            _smartPropertyRepository = smartPropertyRepository;
            _userRepository = userRepository;
            _cycleRepository = cycleRepository;
            _panelRepository = panelRepository;
            _configuration = configuration;
            _mqttClient = client;

            _influxService = influxService;
            _mapper = mapper;
        }

        public async Task<List<SmartDeviceDTO>> GetDevices(Guid userId)
        {
            var devices = await _smartDeviceRepository.FindByCondition(device => device.SmartProperty.User.Id == userId);

            var deviceDtos = new List<SmartDeviceDTO>();
            foreach (var device in devices)
            {
                SmartDeviceDTO deviceDTO = new SmartDeviceDTO();
                deviceDTO.Id = device.Id;
                deviceDTO.Name = device.Name;
                deviceDTO.PathToImage = device.PathToImage;
                if (device.GetType() == typeof(SprinklerSystem))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.SprinklerSystem;
                }
                else if (device.GetType() == typeof(Lamp))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.Lamp;
                }
                else if (device.GetType() == typeof(HouseBattery))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.HouseBattery;
                }
                else if (device.GetType() == typeof(VehicleGate))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.VehicleGate;
                }
                else if (device.GetType() == typeof(ElectricVehicleCharger))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.ElectricVehicleCharger;
                }
                else if (device.GetType() == typeof(SolarPanelSystem))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.SolarPanelSystem;
                }
                else if (device.GetType() == typeof(AirConditioner))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.AirConditioner;
                }
                else if (device.GetType() == typeof(AmbientSensor))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.AmbientSensor;
                }
                else if (device.GetType() == typeof(WashingMachine))
                {
                    deviceDTO.SmartDeviceType = SmartDeviceType.WashingMachine;
                }
                deviceDtos.Add(deviceDTO);
            }
            return deviceDtos;
        }

        public async Task<Result<SmartDeviceDTO>> RegisterAmbientSensor(RegisterDeviceDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new AmbientSensor
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage
            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.AmbientSensor);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterLamp(RegisterLampDTO lamp, Guid userId)
        {
            SmartProperty property = await GetProperty(lamp.SmartPropertyId, userId);

            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new Lamp
            {
                Name = lamp.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = lamp.PowerSupply,
                PowerUsage = lamp.PowerUsage,
                Luminosity = lamp.Luminosity,
            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + lamp.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(lamp.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.Lamp);


            return Result.Ok(newSmartDevice);
        }

        private static async Task SaveDeviceImage(string image, string imagePath)
        {
            byte[] imageBytes = Convert.FromBase64String(image);

            await File.WriteAllBytesAsync(imagePath, imageBytes);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterSprinklerSystem(RegisterDeviceDTO device, Guid userId)
        {

            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);

            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new SprinklerSystem
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage
            };
            _smartDeviceRepository.Create(smartDevice);

            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);

            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.SprinklerSystem);


            return Result.Ok(newSmartDevice);
        }

        private async Task<User?> GetUser(Guid userId)
        {
            return await _userRepository.FindById(userId);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterAirConditioner(RegisterAirConditionerDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new AirConditioner
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage,
                MinTemperature = device.MinTemperature,
                MaxTemperature = device.MaxTemperature,
                Modes = device.Modes,
                CurrentMode = Mode.Automatic
            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.AirConditioner);


            return Result.Ok(newSmartDevice);
        }

        private async Task<SmartProperty> GetProperty(Guid smartPropertyId, Guid userId)
        {
            return await _smartPropertyRepository.FindSingleByCondition(property => property.Id == smartPropertyId && property.User.Id == userId && property.IsAccepted == Activation.Accepted);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterWashingMachine(RegisterWashingMachineDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }
            var cycles = await _cycleRepository.FindByCondition(cycle => device.SupportedCycles.Contains(cycle.Id));
            var smartDevice = new WashingMachine
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage,
                SupportedCycles = cycles.ToList()

            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.WashingMachine);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<ICollection<Cycle>>> GetWashingMachineCycles()
        {
            return Result.Ok(await _cycleRepository.FindAll());
        }

        public async Task<Result<SmartDeviceDTO>> RegisterVehicleGate(RegisterDeviceDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);

            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new VehicleGate
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage,
            };
            _smartDeviceRepository.Create(smartDevice);

            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.VehicleGate);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterHouseBattery(RegisterHouseBatteryDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new HouseBattery
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = TypeOfPowerSupply.Grid,
                PowerUsage = 0,
                BatterySize = device.BatterySize,
                OccupationLevel = 0

            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.HouseBattery);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterElectricVehicleCharger(RegisterElectricChargerDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);

            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }

            var smartDevice = new ElectricVehicleCharger
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage,
                Power = device.Power,
                NumberOfConnections = device.NumberOfConnections,
                PercentageOfCharge = 100.0,
                IsOn = true
            };
            _smartDeviceRepository.Create(smartDevice);

            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);

            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.ElectricVehicleCharger);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<SmartDeviceDTO>> RegisterSolarPanelSystem(RegisterSolarPanelSystemDTO device, Guid userId)
        {
            SmartProperty property = await GetProperty(device.SmartPropertyId, userId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));
            }

            User? user = await GetUser(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));

            }
            var panels = new List<Panel>();
            foreach (var panel in device.Panels)
            {
                var newPanel = new Panel() { Size = panel.Size, Efficency = panel.Efficency };
                _panelRepository.Create(newPanel);
                panels.Add(newPanel);
            }
            var smartDevice = new SolarPanelSystem
            {
                Name = device.Name,
                PathToImage = "",
                IsOnline = false,
                TypeOfPowerSupply = device.PowerSupply,
                PowerUsage = device.PowerUsage,
                Panels = panels

            };
            _smartDeviceRepository.Create(smartDevice);
            var imagePath = "images/devices/" + user.UserName + "_" + smartDevice.Id + "." + device.ImageType;
            smartDevice.PathToImage = imagePath;

            await SaveDeviceImage(device.Image, imagePath);
            property.Devices.Add(smartDevice);
            await _smartPropertyRepository.SaveChanges();

            var newSmartDevice = new SmartDeviceDTO(smartDevice, property.Id, SmartDeviceType.SolarPanelSystem);


            return Result.Ok(newSmartDevice);
        }

        public async Task<Result<bool>> TurnOnOff(Guid deviceId, bool isOn, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == deviceId && device.SmartProperty.User.Id == userId && device.IsOnline);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic($"turn-on-off/{deviceId}")
            .WithPayload(isOn ? "turn_on" : "turn_off")
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
            var user = await _userRepository.FindById(userId);
            if (user != null)
            {
                SmartDeviceType type = GetDeviceType(device.GetType().Name);
                var isOnStr = 1;
                if (!isOn)
                {
                    isOnStr = 0;
                }
                _influxService.Write(write =>
                {

                    var point = PointData.Measurement("deviceonoff")
                        .Tag("deviceId", device.Id.ToString())
                        .Tag("username", user.UserName)
                        .Tag("typeOfDevice", type.ToString())
                        .Field("isOn", isOnStr)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
                if (type == SmartDeviceType.SprinklerSystem)
                {

                    _influxService.Write(write =>
                    {
                        var point = PointData.Measurement("sprinkleEvent")
                    .Tag("deviceId", device.Id.ToString())
                    .Tag("username", user.UserName)
                    .Tag("actionNumber", "1")
                    .Field("value", isOn.ToString())

                    .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                        write.WritePoint(point, "smarthome", "FTN");
                    });

                }
            }
            await _mqttClient.PublishAsync(mqttMessage);
            return Result.Ok(isOn);
        }


        public async Task<Result<bool>> TurnGatePublicPrivate(Guid deviceId, bool isPublic, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == deviceId && device.SmartProperty.User.Id == userId && device.IsOnline);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic($"command/{deviceId}")
            .WithPayload(isPublic ? "turn_public" : "turn_private")
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
            var user = await _userRepository.FindById(userId);
            if (user != null)
            {


                _influxService.Write(write =>
                {
                    var point = PointData.Measurement("deviceonoff")
                        .Tag("deviceId", device.Id.ToString())
                        .Tag("username", user.UserName)
                        .Field("isPublic", isPublic)
                        .Timestamp(DateTime.UtcNow.AddHours(1), InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
            }
            await _mqttClient.PublishAsync(mqttMessage);
            return Result.Ok(isPublic);
        }

        public async Task<Result<bool>> ChangeSprinklerSpecialState(Guid deviceId, bool isActive, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == deviceId && device.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic($"command/{deviceId}")
            .WithPayload(isActive ? "turn_on" : "turn_off")
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
            var user = await _userRepository.FindById(userId);
            if (user != null)
            {
                _influxService.Write(write =>
                {
                    var point = PointData.Measurement("sprinkleEvent")
                                .Tag("deviceId", device.Id.ToString())
                                .Tag("username", user.UserName)
                                .Tag("actionNumber", "2")
                                .Field("value", isActive.ToString())

                                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });
            }
            await _mqttClient.PublishAsync(mqttMessage);
            return Result.Ok(isActive);
        }

        public async Task<Result<SprinklerTimeDTO>> ChangeSprinklerStartTime(Guid deviceId, SprinklerTimeDTO startTime, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == deviceId && device.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            if (device is SprinklerSystem validSprinkler)
            {
                if (TimeSpan.TryParse(startTime.StartTime, out var startTimeValid))
                {
                    var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic($"change-starttime/{deviceId}")
            .WithPayload(startTime.StartTime)
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
                    var user = await _userRepository.FindById(userId);
                    if (user != null)
                    {
                        _influxService.Write(write =>
                        {
                            var point = PointData.Measurement("sprinkleEvent")
                        .Tag("deviceId", device.Id.ToString())
                        .Tag("username", user.UserName)
                        .Tag("actionNumber", "5")
                        .Field("value", startTime.StartTime)

                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                            write.WritePoint(point, "smarthome", "FTN");
                        });
                    }
                    validSprinkler.StartSprinkle = startTimeValid;
                    await _smartDeviceRepository.SaveChanges();
                    await _mqttClient.PublishAsync(mqttMessage);
                    return Result.Ok(startTime);
                }
                else
                {
                    return ResultExtensions.FailBadRequest(ErrorMessages.InvalidTime());
                }


            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }

        }

        public async Task<Result<SprinklerTimeDTO>> ChangeSprinklerEndTime(Guid deviceId, SprinklerTimeDTO endTime, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == deviceId && device.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            if (device is SprinklerSystem validSprinkler)
            {

                if (TimeSpan.TryParse(endTime.StartTime, out var endTimeValid))
                {
                    var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"change-endtime/{deviceId}")
                .WithPayload(endTime.StartTime)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
                    var user = await _userRepository.FindById(userId);
                    if (user != null)
                    {
                        _influxService.Write(write =>
                        {
                            var point = PointData.Measurement("sprinkleEvent")
                                .Tag("deviceId", device.Id.ToString())
                                .Tag("username", user.UserName)
                                .Tag("actionNumber", "6")
                                .Field("value", endTime.StartTime)

                                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                            write.WritePoint(point, "smarthome", "FTN");
                        });
                    }
                    validSprinkler.EndSprinkle = endTimeValid;
                    await _smartDeviceRepository.SaveChanges();
                    await _mqttClient.PublishAsync(mqttMessage);
                    return Result.Ok(endTime);
                }
                else
                {
                    return ResultExtensions.FailBadRequest(ErrorMessages.InvalidTime());
                }

            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }

        }


        public async Task<Result<PagedList<SmartDeviceDTO>>> GetDevicesByPropertyId(Guid propertyId, Guid userId, Page page)

        {
            var devices = await _smartDeviceRepository.FindByCondition(device => device.SmartProperty.Id == propertyId && device.SmartProperty.User.Id == userId && device.SmartProperty.IsAccepted == Activation.Accepted, page);
            var devicesDTO = _mapper.Map<PagedList<SmartDeviceDTO>>(devices);
            for (int i = 0; i < devices.Count; i++)
            {
                SmartDeviceType type = GetDeviceType(devices[i].GetType().Name);
                devicesDTO[i].SmartDeviceType = type;
            }
            return Result.Ok(devicesDTO);

        }

        private SmartDeviceType GetDeviceType(string name)
        {
            var deviceTypes = new Dictionary<string, SmartDeviceType>
            {
                { "Lamp", SmartDeviceType.Lamp},
                { "SprinklerSystem", SmartDeviceType.SprinklerSystem},
                { "VehicleGate", SmartDeviceType.VehicleGate},
                { "ElectricVehicleCharger", SmartDeviceType.ElectricVehicleCharger},
                { "HouseBattery", SmartDeviceType.HouseBattery},
                { "SolarPanelSystem", SmartDeviceType.SolarPanelSystem},
                { "AirConditioner", SmartDeviceType.AirConditioner},
                { "AmbientSensor", SmartDeviceType.AmbientSensor},
                { "WashingMachine", SmartDeviceType.WashingMachine},

            };
            if (deviceTypes.ContainsKey(name))
            {
                return deviceTypes[name];
            }

            return SmartDeviceType.SprinklerSystem;

        }

        public async Task<Result<List<SmartDeviceForPingDTO>>> GetNDevices(int totalDevices)
        {
            var devices = await _smartDeviceRepository.GetNItems(totalDevices);
            var returnDevices = new List<SmartDeviceForPingDTO>();
            foreach (var device in devices)
            {
                var newDevice = new SmartDeviceForPingDTO()
                {
                    Id = device.Id,
                    Name = device.Name,
                    TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                    PowerUsage = device.PowerUsage,
                    IsOn = device.IsOn,
                };
                if (device is Lamp lamp)
                {
                    newDevice.LLuminosity = lamp.Luminosity;
                    newDevice.SmartDeviceType = SmartDeviceType.Lamp.ToString();
                }
                else if (device is VehicleGate vehiclegate)
                {
                    newDevice.VGIsPublic = vehiclegate.IsPublic;
                    newDevice.VGValidLicensePlates = vehiclegate.ValidLicensePlates;

                    newDevice.SmartDeviceType = SmartDeviceType.VehicleGate.ToString();
                }
                else if (device is AirConditioner airConditioner)
                {
                    newDevice.ACMinTemperature = airConditioner.MinTemperature;
                    newDevice.ACMaxTemperature = airConditioner.MaxTemperature;
                    newDevice.ACModes = airConditioner.Modes;
                    newDevice.ACCurrentMode = airConditioner.CurrentMode;

                    newDevice.SmartDeviceType = SmartDeviceType.AirConditioner.ToString();
                }
                else if (device is WashingMachine washingMachine)
                {
                    washingMachine.SupportedCycles = await _smartDeviceRepository.GetWashingMachineCycles(washingMachine.Id);

                    newDevice.WMCurrentCycle = washingMachine.CurrentCycle;

                    newDevice.WMSupportedCycles = new();
                    foreach (var cycle in washingMachine.SupportedCycles)
                    {
                        newDevice.WMSupportedCycles.Add(new()
                        {
                            Name = cycle.Name.ToString(),
                            Temperature = cycle.Temperature,
                            Duration = cycle.Duration,
                        });
                    }

                    newDevice.SmartDeviceType = SmartDeviceType.WashingMachine.ToString();
                }
                else if (device is ElectricVehicleCharger electricVehicleCharger)
                {
                    newDevice.EWNumberOfConnections = electricVehicleCharger.NumberOfConnections;
                    newDevice.EWPower = electricVehicleCharger.Power;
                    newDevice.EWPercentageOfCharge = electricVehicleCharger.PercentageOfCharge;

                    newDevice.SmartDeviceType = SmartDeviceType.ElectricVehicleCharger.ToString();
                }
                else if (device is HouseBattery battery)
                {
                    newDevice.HBBatterySize = battery.BatterySize;
                    newDevice.HBOccupationLevel = battery.OccupationLevel;

                    newDevice.SmartDeviceType = SmartDeviceType.HouseBattery.ToString();
                }
                else if (device is SolarPanelSystem sps)
                {
                    sps.Panels = await _smartDeviceRepository.GetPanels(sps.Id);

                    newDevice.SPSPanels = new();
                    foreach (var cycle in sps.Panels)
                    {
                        newDevice.SPSPanels.Add(new()
                        {
                            Size = cycle.Size,
                            Efficency = cycle.Efficency
                        });
                    }


                    newDevice.SmartDeviceType = SmartDeviceType.SolarPanelSystem.ToString();
                }
                else if (device is SprinklerSystem sprinklerSystem)
                {
                    newDevice.SSIsSpecialMode = sprinklerSystem.IsSpecialMode;
                    newDevice.SSStartSprinkle = sprinklerSystem.StartSprinkle;
                    newDevice.SSEndSprinkle = sprinklerSystem.EndSprinkle;
                    newDevice.SSActiveDays = sprinklerSystem.ActiveDays;

                    newDevice.SmartDeviceType = SmartDeviceType.SprinklerSystem.ToString();
                }
                else if (device is AmbientSensor ambientSensor)
                {
                    newDevice.ASRoomTemperature = ambientSensor.RoomTemperature;
                    newDevice.ASRoomHumidity = ambientSensor.RoomHumidity;
                    newDevice.SmartDeviceType = SmartDeviceType.AmbientSensor.ToString();
                }

                returnDevices.Add(newDevice);

            }
            return Result.Ok(returnDevices);
        }


        public async Task<Result<GateInfoDTO>> GetGateInfo(Guid id, Guid userId)
        {
            SmartDevice gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);

            if (gate == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
            if (gate is VehicleGate vehicleGate)
            {
                GateInfoDTO gateInfoDTO = new GateInfoDTO()
                {
                    isPublic = vehicleGate.IsPublic,
                    isOn = vehicleGate.IsOn,
                    isOnline = vehicleGate.IsOnline,
                };

                return Result.Ok(gateInfoDTO);
            }
            else
            {

                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }

        }

        public async Task<Result<SprinklerInfoDTO>> GetSprinklerInfo(Guid id, Guid userId)
        {
            SmartDevice sprinkler = await _smartDeviceRepository.FindSingleByCondition(sprinkler => sprinkler.Id == id && sprinkler.SmartProperty.User.Id == userId);

            if (sprinkler == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
            if (sprinkler is SprinklerSystem ssprinkler)
            {
                SprinklerInfoDTO sprinklerInfo = new SprinklerInfoDTO()
                {
                    isPublic = ssprinkler.IsSpecialMode,
                    isOn = ssprinkler.IsOn,
                    isOnline = ssprinkler.IsOnline,
                    startTime = ssprinkler.StartSprinkle.ToString(),
                    endTime = ssprinkler.EndSprinkle.ToString()
                };

                return Result.Ok(sprinklerInfo);
            }
            else
            {

                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }

        }

        public async Task<Result<List<String>>> GetLicencePlates(Guid id, Guid userId)
        {
            SmartDevice gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);

            if (gate == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
            if (gate is VehicleGate vehicleGate)
            {
                return Result.Ok(vehicleGate.ValidLicensePlates);
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }

        }

        public async Task<Result<List<String>>> GetActiveDays(Guid id, Guid userId)
        {
            SmartDevice sprinkler = await _smartDeviceRepository.FindSingleByCondition(sprinkler => sprinkler.Id == id && sprinkler.SmartProperty.User.Id == userId);

            if (sprinkler == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
            if (sprinkler is SprinklerSystem mySprinkler)
            {
                return Result.Ok(mySprinkler.ActiveDays);
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }

        }

        public async Task<Result<LicencePlateDTO>> RegisterLicencePlate(Guid id, LicencePlateDTO licencePlate, Guid userId)
        {
            SmartDevice gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);

            if (gate == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
            if (gate is VehicleGate vehicleGate)
            {
                if (vehicleGate.ValidLicensePlates.Contains(licencePlate.plate))
                {
                    return ResultExtensions.FailBadRequest(ErrorMessages.ExistingLicencePlate());

                }

                vehicleGate.ValidLicensePlates.Add(licencePlate.plate);
                var mqttMessage = new MqttApplicationMessageBuilder()
           .WithTopic($"licence-register/{id}")
           .WithPayload(licencePlate.plate)
           .WithExactlyOnceQoS()
           .WithRetainFlag()
           .Build();
                await _mqttClient.PublishAsync(mqttMessage);
                await _smartDeviceRepository.SaveChanges();
                return (licencePlate);
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
        }

        public async Task<Result<string>> RemoveLicencePlate(Guid id, LicencePlateDTO licencePlate, Guid userId)
        {
            SmartDevice gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);

            if (gate == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
            if (gate is VehicleGate vehicleGate)
            {
                if (vehicleGate.ValidLicensePlates.Contains(licencePlate.plate))
                {
                    vehicleGate.ValidLicensePlates.Remove(licencePlate.plate);
                    var mqttMessage = new MqttApplicationMessageBuilder()
           .WithTopic($"licence-delete/{id}")
           .WithPayload(licencePlate.plate)
           .WithExactlyOnceQoS()
           .WithRetainFlag()
           .Build();

                    await _mqttClient.PublishAsync(mqttMessage);
                    await _smartDeviceRepository.SaveChanges();
                    return ("Succesfully removed licence plate");


                }
                return ResultExtensions.FailBadRequest(ErrorMessages.NonExistingLicencePlate());
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("gate", "id"));
            }
        }

        public async Task<Result<DayDTO>> AddSprinklerDay(Guid id, DayDTO day, Guid userId)
        {
            List<string> validDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            SmartDevice sprinkler = await _smartDeviceRepository.FindById(id);
            if (sprinkler == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
            if (!validDays.Contains(day.day))
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.InvalidDay());
            }
            if (sprinkler is SprinklerSystem validSprinkler)
            {
                Console.WriteLine(validSprinkler);
                Console.WriteLine(validSprinkler.ActiveDays);
                if (validSprinkler.ActiveDays.Contains(day.day))
                {


                    return ResultExtensions.FailBadRequest(ErrorMessages.ExistingSprinklerDay());

                }

                validSprinkler.ActiveDays.Add(day.day);
                var mqttMessage = new MqttApplicationMessageBuilder()
           .WithTopic($"day-add/{id}")
           .WithPayload(day.day)
           .WithExactlyOnceQoS()
           .WithRetainFlag()
           .Build();
                var user = await _userRepository.FindById(userId);
                if (user != null)
                {
                    _influxService.Write(write =>
                    {
                        var point = PointData.Measurement("sprinkleEvent")
                            .Tag("deviceId", validSprinkler.Id.ToString())
                            .Tag("username", user.UserName)
                            .Tag("actionNumber", "3")
                            .Field("value", day.day)

                            .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                        write.WritePoint(point, "smarthome", "FTN");
                    });
                }
                await _mqttClient.PublishAsync(mqttMessage);
                await _smartDeviceRepository.SaveChanges();
                return (day);
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
        }

        public async Task<Result<string>> RemoveSprinklerDay(Guid id, DayDTO day, Guid userId)
        {
            SmartDevice sprinkler = await _smartDeviceRepository.FindById(id);
            if (sprinkler == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
            if (sprinkler is SprinklerSystem validSprinkler)
            {
                if (validSprinkler.ActiveDays.Contains(day.day))
                {
                    validSprinkler.ActiveDays.Remove(day.day);
                    var mqttMessage = new MqttApplicationMessageBuilder()
           .WithTopic($"day-delete/{id}")
           .WithPayload(day.day)
           .WithExactlyOnceQoS()
           .WithRetainFlag()
           .Build();
                    var user = await _userRepository.FindById(userId);
                    if (user != null)
                    {
                        _influxService.Write(write =>
                        {
                            var point = PointData.Measurement("sprinkleEvent")
                                .Tag("deviceId", validSprinkler.Id.ToString())
                                .Tag("username", user.UserName)
                                .Tag("actionNumber", "4")
                                .Field("value", day.day)

                                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                            write.WritePoint(point, "smarthome", "FTN");
                        });
                    }
                    await _mqttClient.PublishAsync(mqttMessage);
                    await _smartDeviceRepository.SaveChanges();
                    return ("Succesfully removed sprinkler day");


                }
                return ResultExtensions.FailBadRequest(ErrorMessages.NonExistingSprinklerDay());
            }
            else
            {
                // Transformation failed, throw an error or handle the case accordingly
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("sprinkler", "id"));
            }
        }

        public async Task<Result<BatteryDTO>> GetBattery(Guid batteryId, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(battery => battery.Id == batteryId && battery.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));

            }
            if (device is HouseBattery battery)
            {
                var batteryDTO = _mapper.Map<BatteryDTO>(battery);
                return Result.Ok(batteryDTO);
            }
            return ResultExtensions.FailNotFound(ErrorMessages.NotFound("battery", "id"));

        }

        public async Task<Result<SpSDTO>> GetSps(Guid spsId, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(sps => sps.Id == spsId && sps.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));

            }
            if (device is SolarPanelSystem sps)
            {
                var spsDTO = new SpSDTO()
                {
                    Id = sps.Id,
                    Name = sps.Name,
                    IsOn = sps.IsOn,
                    IsOnline = sps.IsOnline,
                    PathToImage = sps.PathToImage,
                };
                var panels = _mapper.Map<List<PanelDTO>>(await _smartDeviceRepository.GetPanels(device.Id));
                spsDTO.Panels = panels;
                return Result.Ok(spsDTO);
            }
            return ResultExtensions.FailNotFound(ErrorMessages.NotFound("solar panel system", "id"));

        }

        public async Task<Result<double>> SetPercentageOfCharge(Guid id, double percentageOfCharge, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));
            }
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == id && device.SmartProperty.User.Id == userId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("vehicle charger", "id")); }

            if (percentageOfCharge < 1 || percentageOfCharge > 100)
            {
                return ResultExtensions.FailBadRequest("Max Charge Percentage should be between 1 and 100.");
            }

            if (device is ElectricVehicleCharger vhchg)
            {
                vhchg.PercentageOfCharge = percentageOfCharge;
                var mqttMessage = new MqttApplicationMessageBuilder()
               .WithTopic($"vehicle-charger-max-charge/{id}")
               .WithPayload(percentageOfCharge.ToString())
               .WithExactlyOnceQoS()
               .WithRetainFlag()
               .Build();
                await _mqttClient.PublishAsync(mqttMessage);
                await _smartDeviceRepository.SaveChanges();

                _influxService.Write(write =>
                {

                    var point = PointData.Measurement("vehiclecharger")
                    .Tag("deviceId", device.Id.ToString())
                        .Tag("username", user.UserName)
                        .Tag("typeOfDevice", SmartDeviceType.ElectricVehicleCharger.ToString())
                        .Field("percentageOfCharge", percentageOfCharge)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ms);

                    write.WritePoint(point, "smarthome", "FTN");
                });

                return Result.Ok(percentageOfCharge);
            }
            else
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
        }

        public async Task<Result<VehicleChargerDTO>> GetVehicleCharger(Guid vehicleChargerId, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(vhc => vhc.Id == vehicleChargerId && vhc.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));

            }
            if (device is ElectricVehicleCharger evc)
            {
                var vc = new VehicleChargerDTO()
                {
                    Id = evc.Id,
                    Name = evc.Name,
                    IsOn = evc.IsOn,
                    IsOnline = evc.IsOnline,
                    PathToImage = evc.PathToImage,
                    NumberOfConnections = evc.NumberOfConnections,
                    PercentageOfCharge = evc.PercentageOfCharge,
                    Power = evc.Power
                };

                return Result.Ok(vc);
            }
            return ResultExtensions.FailNotFound(ErrorMessages.NotFound("vehicle charger", "id"));
        }

        public async Task<Result<List<SmartDeviceForPingDTO>>> GetNDevicesWithType(int totalDevices, SmartDeviceType type)
        {
            var devicesForPing = new List<SmartDeviceForPingDTO>();
            if (type == SmartDeviceType.AirConditioner)
            {
                var devices = await _smartDeviceRepository.GetACs(totalDevices);
                foreach (var device in devices)
                {

                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.ACMinTemperature = device.MinTemperature;
                    newDevice.ACMaxTemperature = device.MaxTemperature;
                    newDevice.ACModes = device.Modes;
                    newDevice.ACCurrentMode = device.CurrentMode;
                    newDevice.SmartDeviceType = SmartDeviceType.AirConditioner.ToString();
                    devicesForPing.Add(newDevice);
                }
            }
            else if (type == SmartDeviceType.Lamp)
            {
                var devices = await _smartDeviceRepository.GetLamps(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.LLuminosity = device.Luminosity;
                    newDevice.SmartDeviceType = SmartDeviceType.Lamp.ToString();
                    devicesForPing.Add(newDevice);
                }

            }
            else if (type == SmartDeviceType.SolarPanelSystem)
            {
                var devices = await _smartDeviceRepository.GetSolarPanelSystems(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.SPSPanels = new();
                    foreach (var cycle in device.Panels)
                    {
                        newDevice.SPSPanels.Add(new()
                        {
                            Size = cycle.Size,
                            Efficency = cycle.Efficency
                        });
                    }
                    newDevice.SmartDeviceType = SmartDeviceType.SolarPanelSystem.ToString();

                    devicesForPing.Add(newDevice);

                }

            }
            else if (type == SmartDeviceType.SprinklerSystem)
            {
                var devices = await _smartDeviceRepository.GetSprinklerSystems(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.SSIsSpecialMode = device.IsSpecialMode;
                    newDevice.SSStartSprinkle = device.StartSprinkle;
                    newDevice.SSEndSprinkle = device.EndSprinkle;
                    newDevice.SSActiveDays = device.ActiveDays;

                    newDevice.SmartDeviceType = SmartDeviceType.SprinklerSystem.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            else if (type == SmartDeviceType.ElectricVehicleCharger)
            {
                var devices = await _smartDeviceRepository.GetEVChargers(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.EWNumberOfConnections = device.NumberOfConnections;
                    newDevice.EWPower = device.Power;
                    newDevice.EWPercentageOfCharge = device.PercentageOfCharge;

                    newDevice.SmartDeviceType = SmartDeviceType.ElectricVehicleCharger.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            else if (type == SmartDeviceType.AmbientSensor)
            {
                var devices = await _smartDeviceRepository.GetAmbientSensors(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.ASRoomTemperature = device.RoomTemperature;
                    newDevice.ASRoomHumidity = device.RoomHumidity;
                    newDevice.SmartDeviceType = SmartDeviceType.AmbientSensor.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            else if (type == SmartDeviceType.HouseBattery)
            {
                var devices = await _smartDeviceRepository.GetHouseBatteries(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.HBBatterySize = device.BatterySize;
                    newDevice.HBOccupationLevel = device.OccupationLevel;

                    newDevice.SmartDeviceType = SmartDeviceType.HouseBattery.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            else if (type == SmartDeviceType.WashingMachine)
            {
                var devices = await _smartDeviceRepository.GetWashingMachines(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.WMSupportedCycles = new();
                    foreach (var cycle in device.SupportedCycles)
                    {
                        newDevice.WMSupportedCycles.Add(new()
                        {
                            Name = cycle.Name.ToString(),
                            Temperature = cycle.Temperature,
                            Duration = cycle.Duration,
                        });
                    }

                    newDevice.SmartDeviceType = SmartDeviceType.WashingMachine.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            else if (type == SmartDeviceType.VehicleGate)
            {
                var devices = await _smartDeviceRepository.GetVehicleGates(totalDevices);
                foreach (var device in devices)
                {
                    var newDevice = new SmartDeviceForPingDTO()
                    {
                        Id = device.Id,
                        Name = device.Name,
                        TypeOfPowerSupply = device.TypeOfPowerSupply.ToString(),
                        PowerUsage = device.PowerUsage,
                        IsOn = device.IsOn,
                    };
                    newDevice.VGIsPublic = device.IsPublic;
                    newDevice.VGValidLicensePlates = device.ValidLicensePlates;

                    newDevice.SmartDeviceType = SmartDeviceType.VehicleGate.ToString();
                    devicesForPing.Add(newDevice);

                }
            }
            return devicesForPing;
        }
    }
}
