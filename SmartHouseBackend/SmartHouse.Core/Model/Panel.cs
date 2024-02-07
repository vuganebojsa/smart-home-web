using SmartHouse.Core.Model.ElectromagneticDevices;

namespace SmartHouse.Core.Model
{
    public class Panel
    {
        public Guid Id { get; set; }
        public double Size { get; set; }
        public double Efficency { get; set; }
        public Guid SolarPanelSystemId { get; set; }
        public SolarPanelSystem? SolarPanelSystem { get; set; }
    }
}
