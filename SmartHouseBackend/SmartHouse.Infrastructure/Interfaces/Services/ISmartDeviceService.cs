using FluentResults;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;

namespace SmartHouse.Infrastructure.Interfaces.Services
{
    public interface ISmartDeviceService
    {

        Task<Result<GateInfoDTO>> GetGateInfo(Guid id, Guid userId);

        Task<Result<BatteryDTO>> GetBattery(Guid batteryId, Guid userId);

        Task<List<SmartDeviceDTO>> GetDevices(Guid userId);
        Task<Result<PagedList<SmartDeviceDTO>>> GetDevicesByPropertyId(Guid propertyId, Guid userId, Page page);
        Task<Result<List<SmartDeviceForPingDTO>>> GetNDevices(int totalDevices);
        Task<Result<SpSDTO>> GetSps(Guid spsId, Guid userId);
        Task<Result<ICollection<Cycle>>> GetWashingMachineCycles();
        Task<Result<SmartDeviceDTO>> RegisterAirConditioner(RegisterAirConditionerDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterAmbientSensor(RegisterDeviceDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterElectricVehicleCharger(RegisterElectricChargerDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterHouseBattery(RegisterHouseBatteryDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterLamp(RegisterLampDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterSolarPanelSystem(RegisterSolarPanelSystemDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterSprinklerSystem(RegisterDeviceDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterVehicleGate(RegisterDeviceDTO device, Guid userId);
        Task<Result<SmartDeviceDTO>> RegisterWashingMachine(RegisterWashingMachineDTO device, Guid userId);
        Task<Result<bool>> TurnGatePublicPrivate(Guid deviceId, bool isPublic, Guid userId);
        Task<Result<bool>> TurnOnOff(Guid deviceId, bool isOn, Guid userId);

        Task<Result<List<string>>> GetLicencePlates(Guid id, Guid userId);
        Task<Result<LicencePlateDTO>> RegisterLicencePlate(Guid id, LicencePlateDTO licencePlate, Guid userId);
        Task <Result<string>> RemoveLicencePlate(Guid id, LicencePlateDTO licencePlate, Guid userId);
        Task<Result<DayDTO>> AddSprinklerDay(Guid id, DayDTO day, Guid userId);

        Task<Result<string>> RemoveSprinklerDay(Guid id, DayDTO day, Guid userId);
        Task<Result<List<string>>> GetActiveDays(Guid id, Guid userId);
        Task<Result<bool>> ChangeSprinklerSpecialState(Guid deviceId, bool isActive, Guid userId);
        Task<Result<SprinklerTimeDTO>> ChangeSprinklerStartTime(Guid id, SprinklerTimeDTO startTime, Guid value);
        Task<Result<SprinklerTimeDTO>> ChangeSprinklerEndTime(Guid id, SprinklerTimeDTO endTime, Guid value);
        Task<Result<SprinklerInfoDTO>> GetSprinklerInfo(Guid id, Guid userId);
        Task<Result<double>> SetPercentageOfCharge(Guid id, double percentageOfCharge, Guid userId);
        Task<Result<VehicleChargerDTO>> GetVehicleCharger(Guid vehicleChargerId, Guid userId);
        Task<Result<List<SmartDeviceForPingDTO>>> GetNDevicesWithType(int totalDevices, SmartDeviceType type);
    }
}
