namespace SmartHouse.Infrastructure.DTOS
{
    public class VehicleChargerAllActionsDTO
    {
        public string Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Executer { get; set; }
        public string Action { get; set; }
        public VehicleChargerAllActionsDTO(string value, DateTime timeStamp, string executer, string action)
        {
            TimeStamp = timeStamp;
            Value = value;
            Executer = executer;
            Action = action;
        }
    }
}
