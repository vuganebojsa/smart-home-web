using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.DTOS
{
    public class AdminProperties
    {
        public Guid Id { get; set; }
        public TypeOfProperty TypeOfProperty { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string OwnerName { get; set; }

        public AdminProperties()
        {

        }
    }
}
