using FluentResults;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.InfluxDTOs;
using SmartHouse.Infrastructure.MqttDTOs;

namespace SmartHouse.Infrastructure.Interfaces.Services
{
    public interface IReportService
    {
        Task<Result<List<SPSActionsDTO>>> GetSolarPanelSystemHistory(Guid spsId, string username, string from, string to, Guid userId);
        Task<Result<List<LuminosityDataDTO>>> GetLampLuminosityHistory(Guid id, string startDate, string endDate, Guid userId);

        Task<Result<List<GateEventInfoDTO>>> GetGateEventHistory(Guid id, string startDate, string endDate, string licencePlate, Guid userId);
        Task<Result<List<DeviceOnOffInfoDTO>>> GetDeviceOnOffHistory(Guid id, string startDate, string endDate, Guid userId);
        Task<Result<List<GatePublicPrivateInfoDTO>>> GetGatePublicPrivateHistory(Guid id, string startDate, string endDate, Guid userId);


        Task<Result<List<PropertyConsumptionDTO>>> GetEnergyConsumption(Guid smartPropertyId, TotalTimePeriod totalTimePeriod, Guid userId);
        Task<Result<List<PropertyConsumptionDTO>>> GetPropertyEnergyConsumption(Guid smartPropertyId, Guid userId);
        Task<Result<List<PropertyConsumptionDTO>>> GetEnergyConsumptionFromTo(Guid smartPropertyId, string from, string to, Guid userId);



        Task<Result<List<TemperatureDTO>>> GetTemperatureData(Guid smartDeviceId, Guid userId);
        Task<Result<List<TemperatureDTO>>> GetTemperatureTimePeriod(Guid smartPropertyId, TotalTimePeriod totalTimePeriod, Guid value);
        Task<Result<List<TemperatureDTO>>> GetTemperatureFromTo(Guid smartDeviceId, string from, string to, Guid userId);
        Task<Result<List<HumidityDTO>>> GetHumidityData(Guid smartDeviceId, Guid userId);
        Task<Result<List<HumidityDTO>>> GetHumidityTimePeriod(Guid smartDeviceId, TotalTimePeriod totalTimePeriod, Guid userId);
        Task<Result<List<HumidityDTO>>> GetHumidityFromTo(Guid smartDeviceId, string from, string to, Guid userId);
        Task<Result<OnlineOfflineHistoryDTO>> GetDeviceOnlineOfflineHistory(Guid id, string startDate, string endDate, Guid userId);
        Task<Result<List<SprinklerEventInfoDTO>>> GetSprinklerEventHistory(Guid id, string startDate, string endDate, string username, Guid userId);
        Task<Result<List<VehicleChargerActionsDTO>>> GetVehicleChargerHistory(Guid id, Guid userId);

        Task<Result<List<VehicleChargerAllActionsDTO>>> GetAllActionsByVehicleChargerInRange(Guid id, string from, string to, Guid userId, string executer);
        Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByCity(string cityName, TotalTimePeriod totalTimePeriod, Guid adminId, bool isConsumed);
        Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByCityInRange(string cityName, string from, string to, Guid adminId, bool isConsumed);
        Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByProperty(Guid propertyId, TotalTimePeriod totalTimePeriod, Guid value, bool isConsumed);
        Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByPropertyInRange(Guid propertyId, string from, string to, Guid value, bool isConsumed);
    }
}
