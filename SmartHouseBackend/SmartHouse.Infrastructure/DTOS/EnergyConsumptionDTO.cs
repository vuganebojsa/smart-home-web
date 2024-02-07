namespace SmartHouse.Infrastructure.DTOS
{
    public class EnergyConsumptionDTO
    {
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public EnergyConsumptionDTO(DateTime timestamp, double value)
        {
            TimeStamp = timestamp;
            Value = value;
        }

    }
}
