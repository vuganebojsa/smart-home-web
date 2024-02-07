using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHouse.Core.Model.OutsideSmartDevices
{
    public class SprinklerSystem : SmartDevice
    {
        public bool IsSpecialMode { get; set; }
        public TimeSpan StartSprinkle { get; set; }
        public TimeSpan EndSprinkle { get; set; }
        
        public List<string> ActiveDays { get; set; } = new();
    }
}
