namespace SmartHouse.Core.Model.ElectromagneticDevices
{
    public class ElectricVehicleCharger : SmartDevice
    {
        public int NumberOfConnections { get; set; } = 0;
        public double Power { get; set; } = 0.0;
        public double PercentageOfCharge { get; set; } = 100.0;
    }
}
