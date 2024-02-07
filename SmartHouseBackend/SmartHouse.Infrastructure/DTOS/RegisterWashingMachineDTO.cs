using SmartHouse.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterWashingMachineDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name should be at least 3 characters long")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Smart Property Id is required.")]
        public Guid SmartPropertyId { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Image type is required")]
        public string ImageType { get; set; } = "jpg";

        [Required(ErrorMessage = "Type of power supply is required.")]
        public TypeOfPowerSupply PowerSupply { get; set; }
        public double PowerUsage { get; set; } = 0;

        [Required(ErrorMessage = "Atleast one Cycle is required")]
        [MinLength(1, ErrorMessage = "Atleast one Cycle is required")]
        public List<Guid> SupportedCycles { get; set; } = new();
    }
}
