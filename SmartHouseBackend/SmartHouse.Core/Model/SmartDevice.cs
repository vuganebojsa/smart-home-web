namespace SmartHouse.Core.Model
{
    public enum TypeOfPowerSupply
    {
        Battery, Grid
    }
    public class SmartDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PathToImage { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsOn { get; set; } = false;
        public SmartProperty? SmartProperty { get; set; }
        public TypeOfPowerSupply TypeOfPowerSupply { get; set; }
        public double PowerUsage { get; set; } = 0;
        public DateTime LastPingTime { get; set; } = DateTime.MinValue;
    }
}
