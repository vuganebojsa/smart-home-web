using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterHouseBatteryDTO : RegisterDeviceDTO
    {

        [Required(ErrorMessage = "Battery Size is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value that is positive.")]
        public double BatterySize { get; set; }
    }
}
