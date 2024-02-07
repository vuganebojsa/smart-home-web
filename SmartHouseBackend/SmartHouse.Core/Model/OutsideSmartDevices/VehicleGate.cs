namespace SmartHouse.Core.Model.OutsideSmartDevices
{
    public class VehicleGate : SmartDevice
    {
        public bool IsPublic { get; set; } = true;
        public List<string> ValidLicensePlates { get; set; } = new();
    }
}
