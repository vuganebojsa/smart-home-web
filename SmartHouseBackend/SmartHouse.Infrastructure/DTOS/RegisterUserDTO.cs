using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartHouse.DTOS
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Username is required.")]

        [MinLength(5, ErrorMessage = "Username should be at least 5 characters long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not in the correct format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password should be at least 6 characters long")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Repeat password is required")]
        [MinLength(6, ErrorMessage = "Repeat password should be at least 6 characters long")]
        public string RepeatPassword { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public string Image {  get; set; }

        public string ImageType { get; set; } = "jpg";

        
    }
}
