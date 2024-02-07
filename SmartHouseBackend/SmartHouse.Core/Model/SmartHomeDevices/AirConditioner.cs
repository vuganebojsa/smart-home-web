namespace SmartHouse.Core.Model.SmartHomeDevices
{
    public enum Mode
    {
        Cooling,
        Heating,
        Automatic,
        Ventilation
    }
    public class AirConditioner : SmartDevice
    {
        public double MinTemperature { get; set; } = 14;
        public double MaxTemperature { get; set; } = 30;
        public List<Mode> Modes { get; set; } = new();
        public Mode CurrentMode { get; set; } = Mode.Automatic;
    }
}
