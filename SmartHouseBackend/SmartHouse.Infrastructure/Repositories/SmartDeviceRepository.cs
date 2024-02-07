using Microsoft.EntityFrameworkCore;
using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Core.Model.OutsideSmartDevices;
using SmartHouse.Core.Model.SmartHomeDevices;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Repositories
{
    public class SmartDeviceRepository : RepositoryBase<SmartDevice>, ISmartDeviceRepository
    {
        public SmartDeviceRepository(DataContext dataContext) : base(dataContext)
        {

        }

        public async Task<ICollection<SmartDevice>> FindByConditionIncludesProperty(Expression<Func<SmartDevice, bool>> expression)
        {
            return await _dataContext.SmartDevices.Where(expression).Include(sp => sp.SmartProperty).ToListAsync();
        }

        public async Task<SmartDevice> FindByIdWithProperty(Expression<Func<SmartDevice, bool>> expression)
        {
            return await _dataContext.SmartDevices.Where(expression).Include(sp => sp.SmartProperty).FirstOrDefaultAsync();
        }

        public async Task<List<HouseBattery>> GetBatteriesByProperty(Guid propertyId)
        {
            return await _dataContext.SmartDevices.OfType<HouseBattery>()
                .Where(device => device.SmartProperty.Id == propertyId && device.IsOnline == true).Take(100).ToListAsync();
        }

        public async Task<VehicleGate> GetVehicleGateById(Guid gateId)
        {
            return await _dataContext.SmartDevices.OfType<VehicleGate>()
                .Where(device => device.Id == gateId && device.IsOnline).FirstOrDefaultAsync();
        }

        public async Task<SprinklerSystem> GetSprinklerById(Guid sprinklerId)
        {
            return await _dataContext.SmartDevices.OfType<SprinklerSystem>()
                .Where(device => device.Id == sprinklerId && device.IsOnline).FirstOrDefaultAsync();
        }

        public async Task<List<Panel>> GetPanels(Guid id)
        {
            return await _dataContext.SmartDevices.OfType<SolarPanelSystem>().
                Where(sps => sps.Id == id).SelectMany(sps => sps.Panels).ToListAsync();
        }

        public async Task<SmartProperty> GetPropertyByDeviceId(Guid deviceId)
        {
            return await _dataContext.SmartProperties.FirstOrDefaultAsync(sp => sp.Devices.Any(d => d.Id == deviceId));
        }

        public async Task<double> GetTotalBateryCapacityByPropertyId(Guid propertyId)
        {
            var capacities = await _dataContext.SmartDevices.OfType<HouseBattery>()
                .Where(battery => battery.SmartProperty.Id == propertyId && battery.IsOnline == true).Select(battery => battery.OccupationLevel).ToListAsync();
            return capacities.Sum();
        }

        public async Task<double> GetTotalEnergyConsumptionByPropertyId(Guid propertyId)
        {
            var consumption = await _dataContext.SmartDevices
                .Where(device => device.TypeOfPowerSupply == TypeOfPowerSupply.Grid && device.SmartProperty.Id == propertyId && device.IsOnline == true).Select(device => device.PowerUsage).ToListAsync();

            return consumption.Sum();
        }

        public async Task<User> GetUserByDeviceId(Guid deviceId)
        {
            var user = await _dataContext.Users
           .FirstOrDefaultAsync(u => u.Properties
               .Any(sp => sp.Devices.Any(d => d.Id == deviceId)));
            return user;

        }

        public async Task<List<Cycle>> GetWashingMachineCycles(Guid id)
        {
            return await _dataContext.SmartDevices.OfType<WashingMachine>().Where(wm => wm.Id == id).SelectMany(wm => wm.SupportedCycles).ToListAsync();
        }

        public async Task<List<AmbientSensor>> GetAmbientSensors(int total)
        {
            return await _dataContext.SmartDevices.OfType<AmbientSensor>().Take(total).ToListAsync();
        }

        public async Task<List<ElectricVehicleCharger>> GetEVChargers(int total)
        {
            return await _dataContext.SmartDevices.OfType<ElectricVehicleCharger>().Take(total).ToListAsync();
        }

        public async Task<List<HouseBattery>> GetHouseBatteries(int total)
        {
            return await _dataContext.SmartDevices.OfType<HouseBattery>().Take(total).ToListAsync();
        }

        public async Task<List<SolarPanelSystem>> GetSolarPanelSystems(int total)
        {
            return await _dataContext.SmartDevices.OfType<SolarPanelSystem>().Include(sps => sps.Panels).Take(total).ToListAsync();
        }

        public async Task<List<Lamp>> GetLamps(int total)
        {
            return await _dataContext.SmartDevices.OfType<Lamp>().Take(total).ToListAsync();
        }

        public async Task<List<SprinklerSystem>> GetSprinklerSystems(int total)
        {
            return await _dataContext.SmartDevices.OfType<SprinklerSystem>().Take(total).ToListAsync();
        }

        public async Task<List<VehicleGate>> GetVehicleGates(int total)
        {
            return await _dataContext.SmartDevices.OfType<VehicleGate>().Take(total).ToListAsync();
        }

        public async Task<List<AirConditioner>> GetACs(int total)
        {
            return await _dataContext.SmartDevices.OfType<AirConditioner>().Take(total).ToListAsync();
        }

        public async Task<List<WashingMachine>> GetWashingMachines(int total)
        {
            return await _dataContext.SmartDevices.OfType<WashingMachine>().Include(wm => wm.CurrentCycle).Include(wm => wm.SupportedCycles).Take(total).ToListAsync();
        }
    }
}
