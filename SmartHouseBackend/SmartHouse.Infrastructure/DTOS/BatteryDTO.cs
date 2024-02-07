namespace SmartHouse.Infrastructure.DTOS
{
    public class BatteryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PathToImage { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsOn { get; set; } = false;
        public double BatterySize { get; set; }
        public double OccupationLevel { get; set; }

        public BatteryDTO()
        {

        }
    }
}
