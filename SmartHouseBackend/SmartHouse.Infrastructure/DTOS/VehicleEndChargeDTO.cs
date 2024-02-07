namespace SmartHouse.Infrastructure.DTOS
{
    public class VehicleEndChargeDTO
    {
        public string DeviceId { get; set; }
        public string Plate { get; set; }
        public double TotalKwhConsumed { get; set; }
        public double MinutesNeeded { get; set; }
        public int CurrentConnections { get; set; }
        public string? Status { get; set; } = "End";

    }
}
