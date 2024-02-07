namespace SmartHouse.Core.Model.ElectromagneticDevices
{
    public class SolarPanelSystem : SmartDevice
    {
        public List<Panel> Panels { get; set; } = new();
    }
}
