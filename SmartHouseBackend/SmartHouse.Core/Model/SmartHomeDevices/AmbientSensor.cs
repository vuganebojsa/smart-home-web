namespace SmartHouse.Core.Model.SmartHomeDevices
{
    public class AmbientSensor : SmartDevice
    {
        public double RoomTemperature { get; set; } = 0;
        public double RoomHumidity { get; set; } = 0;   
    }
}
