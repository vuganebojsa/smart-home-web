using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.DTOS
{
    public class UserProfileDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.USER;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePicturePath { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
    }
}
