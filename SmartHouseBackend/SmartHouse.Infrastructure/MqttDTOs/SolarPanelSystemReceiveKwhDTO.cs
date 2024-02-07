namespace SmartHouse.Infrastructure.MqttDTOs
{
    public class SolarPanelSystemReceiveKwhDTO
    {
        public string DeviceId { get; set; }
        public double TotalKwhPerMinute { get; set; }
    }
}
