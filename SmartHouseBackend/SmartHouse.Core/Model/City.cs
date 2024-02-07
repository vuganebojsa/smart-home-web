namespace SmartHouse.Core.Model
{
    public class City
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid CountryId { get; set; }
        public Country? Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
