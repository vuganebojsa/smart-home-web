namespace SmartHouse.Infrastructure.DTOS
{
    public class VehicleStartChargeDTO
    {
        public string DeviceId { get; set; }
        public string Plate { get; set; }
        public double ToCharge { get; set; }
        public double MinutesNeeded { get; set; }
        public int CurrentConnections { get; set; }
        public string? Status { get; set; } = "Start";
    }
}
