using System.ComponentModel.DataAnnotations;

namespace SmartHouse.DTOS
{
    public class LoginDTO
    {
        [Required]
        [MinLength(4, ErrorMessage = "The username or email should be atleast 4 characters long.")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The password should be atleast 6 characters long.")]
        public string Password { get; set; }
    }
}
