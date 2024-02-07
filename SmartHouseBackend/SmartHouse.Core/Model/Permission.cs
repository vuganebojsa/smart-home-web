namespace SmartHouse.Core.Model
{
    public class Permission
    {
        public Guid Id { get; set; }
        public User Owner { get; set; }
        public User Recipient { get; set; }
        public SmartDevice Device { get; set; }
        public bool IsValid { get; set; }
        public DateTime GrantDate { get; set; }
    }
}
