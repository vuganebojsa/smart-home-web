namespace SmartHouse.Infrastructure.DTOS
{
    public class VehicleChargerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PathToImage { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsOn { get; set; } = false;
        public int NumberOfConnections { get; set; } = 0;
        public double Power { get; set; } = 0.0;
        public double PercentageOfCharge { get; set; } = 100.0;
    }
}
