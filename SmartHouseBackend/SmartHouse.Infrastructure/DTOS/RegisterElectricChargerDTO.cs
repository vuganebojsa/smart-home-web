using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterElectricChargerDTO : RegisterDeviceDTO
    {

        [Required(ErrorMessage = "Battery Size is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value that is positive.")]
        public int NumberOfConnections { get; set; }
        [Required(ErrorMessage = "Battery Size is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value that is positive.")]
        public double Power { get; set; }
    }
}
