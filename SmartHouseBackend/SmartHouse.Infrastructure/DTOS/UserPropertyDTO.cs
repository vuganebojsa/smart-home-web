using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.DTOS
{

    public class UserPropertyDTO
    {
        public Guid Id { get; set; }
        public TypeOfProperty TypeOfProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Activation IsAccepted { get; set; }

        public UserPropertyDTO() { }

        public UserPropertyDTO(SmartProperty property)
        {
            Id = property.Id;
            Country = property.Country;
            City = property.City;
            TypeOfProperty = property.TypeOfProperty;
            Name = property.Name;
            Address = property.Address;
            IsAccepted = property.IsAccepted;


        }
    }
}