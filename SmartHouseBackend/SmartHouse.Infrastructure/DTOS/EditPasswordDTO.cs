using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class EditPasswordDTO
    {
        [Required(ErrorMessage = "Please enter the old password.")]

        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Please enter the new password.")]
        [MinLength(6, ErrorMessage = "New password should be at least 6 characters long")]

        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter the repeat new password.")]
        [MinLength(6, ErrorMessage = "Repeat password should be at least 6 characters long")]

        public string RepeatNewPassword { get; set; }
    }
}
