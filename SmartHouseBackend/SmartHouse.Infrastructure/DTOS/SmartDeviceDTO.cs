using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SmartDeviceDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PathToImage { get; set; } = string.Empty;
        public Guid? SmartPropertyId { get; set; }
        public SmartDeviceType? SmartDeviceType { get; set; }
        public bool? IsOnline { get; set; } = false;
        public bool? IsOn { get; set; } = false;

        public SmartDeviceDTO() { }

        public SmartDeviceDTO(SmartDevice device, Guid smartPropertyId, SmartDeviceType smartDeviceType)
        {
            Id = device.Id;
            Name = device.Name;
            PathToImage = device.PathToImage;
            SmartPropertyId = smartPropertyId;
            SmartDeviceType = smartDeviceType;
        }

        public SmartDeviceDTO(SmartDevice device)
        {
            Id = device.Id;
            Name = device.Name;
            PathToImage = device.PathToImage;
        }
    }
}
