namespace SmartHouse.Core.Model
{
    public enum TypeOfProperty
    {
        House, Apartment
    }
    public enum Activation
    {
        Accepted, Rejected, Pending
    }
    public class SmartProperty
    {
        public Guid Id { get; set; }
        public TypeOfProperty TypeOfProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double Quadrature { get; set; }
        public Activation IsAccepted { get; set; }
        public int NumberOfFloors { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PathToImage { get; set; } = string.Empty;

        public User User { get; set; }
        public List<SmartDevice> Devices { get; set; } = new();
        public string? Reason { get; set; } = string.Empty;
    }
}
