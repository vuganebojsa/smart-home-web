namespace SmartHouse.Core.Model.SmartHomeDevices
{
    public class WashingMachine : SmartDevice
    {
        public Cycle? CurrentCycle { get; set; }
        public List<Cycle> SupportedCycles { get; set; } = new();
    }
}
