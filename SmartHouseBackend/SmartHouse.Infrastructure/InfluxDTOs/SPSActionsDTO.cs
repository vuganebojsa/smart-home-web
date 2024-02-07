namespace SmartHouse.Infrastructure.InfluxDTOs
{
    public class SPSActionsDTO
    {
        public bool IsOn { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Username { get; set; }
        public SPSActionsDTO(double isOn, DateTime timeStamp, string username)
        {
            TimeStamp = timeStamp;
            IsOn = true;
            if (isOn < 1) { IsOn = false; }
            Username = username;
        }
    }
}
