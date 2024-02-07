using SmartHouse.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterLampDTO
    {
        [Required(ErrorMessage = "The device name is required.")]
        public string Name { get; set; }
        public double Luminosity { get; set; } = 100;
        [Required(ErrorMessage = "The power supply is required.")]
        public TypeOfPowerSupply PowerSupply { get; set; }

        [Range(0, Double.MaxValue, ErrorMessage = "Minimum power usage is 0")]
        public double PowerUsage { get; set; } = 0;
        [Required(ErrorMessage = "Smart Property Id is required")]
        public Guid SmartPropertyId { get; set; }
        [Required(ErrorMessage = "Property Image is required")]
        public string Image { get; set; }
        public string ImageType { get; set; } = "jpg";
    }
}
