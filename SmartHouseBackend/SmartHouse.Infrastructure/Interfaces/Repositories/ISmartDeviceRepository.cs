using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Core.Model.OutsideSmartDevices;
using SmartHouse.Core.Model.SmartHomeDevices;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Interfaces.Repositories
{

    public interface ISmartDeviceRepository : IRepositoryBase<SmartDevice>
    {
        public Task<SmartDevice> FindByIdWithProperty(Expression<Func<SmartDevice, bool>> expression);
        public Task<ICollection<SmartDevice>> FindByConditionIncludesProperty(Expression<Func<SmartDevice, bool>> expression);

        public Task<List<Cycle>> GetWashingMachineCycles(Guid id);
        public Task<List<Panel>> GetPanels(Guid id);
        public Task<User> GetUserByDeviceId(Guid deviceId);
        public Task<SmartProperty> GetPropertyByDeviceId(Guid deviceId);
        public Task<List<HouseBattery>> GetBatteriesByProperty(Guid propertyId);


        public Task<VehicleGate> GetVehicleGateById(Guid gateId);

        public Task<SprinklerSystem> GetSprinklerById(Guid sprinklerId);

        public Task<double> GetTotalEnergyConsumptionByPropertyId(Guid propertyId);
        public Task<double> GetTotalBateryCapacityByPropertyId(Guid propertyId);
        public Task<List<AmbientSensor>> GetAmbientSensors(int total);
        public Task<List<ElectricVehicleCharger>> GetEVChargers(int total);
        public Task<List<HouseBattery>> GetHouseBatteries(int total);
        public Task<List<SolarPanelSystem>> GetSolarPanelSystems(int total);
        public Task<List<Lamp>> GetLamps(int total);
        public Task<List<SprinklerSystem>> GetSprinklerSystems(int total);
        public Task<List<VehicleGate>> GetVehicleGates(int total);
        public Task<List<AirConditioner>> GetACs(int total);
        public Task<List<WashingMachine>> GetWashingMachines(int total);

    }

}
