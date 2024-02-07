namespace SmartHouse.Infrastructure.DTOS
{
    public class VehicleChargerActionsDTO
    {
        public double Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Username { get; set; }
        public VehicleChargerActionsDTO(double value, DateTime timeStamp, string username)
        {
            TimeStamp = timeStamp;
            Value = value;
            Username = username;
        }
    }
}
