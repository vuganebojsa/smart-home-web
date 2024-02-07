using SmartHouse.Core.Model;
using SmartHouse.Core.Model.SmartHomeDevices;
using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterAirConditionerDTO
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

        [Required(ErrorMessage = "Minimum Temperature is required.")]
        public double MinTemperature { get; set; } = 14;

        [Required(ErrorMessage = "Maximum Temperature is required.")]
        public double MaxTemperature { get; set; } = 30;

        [Required(ErrorMessage = "Atleast 1 mode is required.")]
        public List<Mode> Modes { get; set; } = new();
    }
}
