namespace SmartHouse.Core.Model
{
    public enum Role
    {
        SUPERADMIN, ADMIN, USER
    }
    public class User
    {
        public Guid Id { get; set; }

        public Role Role { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public string ProfilePicturePath { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public List<SmartProperty>? Properties { get; set; } = new();

        public bool IsPasswordChanged { get; set; } = true;

    }
}
