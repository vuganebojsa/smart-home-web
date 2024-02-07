namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterSolarPanelSystemDTO : RegisterDeviceDTO
    {
        public List<PanelDTO> Panels { get; set; } = new();
    }
}
