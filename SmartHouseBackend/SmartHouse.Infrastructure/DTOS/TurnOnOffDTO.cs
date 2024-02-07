using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class TurnOnOffDTO
    {
        [Required(ErrorMessage = "You need to set the status which you want for the device.")]
        public bool IsOn { get; set; } = false;
    }
}
