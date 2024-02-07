using System.ComponentModel.DataAnnotations;

namespace SmartHouse.Infrastructure.DTOS
{
    public class EditUserDTO
    {
        [Required(ErrorMessage = "Please enter the username.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter the name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the last name.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter the profile picture.")]
        public string ProfilePicture { get; set; }
        [Required(ErrorMessage = "Please enter the type of image.")]
        public string TypeOfImage { get; set; } = "jpg";

        [Required(ErrorMessage = "Please enter the email address.")]
        [EmailAddress(ErrorMessage = "Email not in correct format.")]
        public string Email { get; set; }

    }
}
