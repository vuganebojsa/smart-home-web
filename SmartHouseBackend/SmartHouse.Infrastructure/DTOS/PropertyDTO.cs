using SmartHouse.Core.Model;
using SmartHouse.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class PropertyDTO
    {
        public Guid Id { get; set; }
        public TypeOfProperty TypeOfProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double Quadrature { get; set; }
        public int NumberOfFloors { get; set; }

        public string userName { get; set; }


        public PropertyDTO(SmartProperty property)
        {
            Id = property.Id;
            TypeOfProperty = property.TypeOfProperty;
            Name = property.Name;
            Address = property.Address;
            City = property.City;
            Country = property.Country;
            Quadrature = property.Quadrature;
            NumberOfFloors = property.NumberOfFloors;
            if (property.User != null)
            {
                userName = property.User.UserName;
            }

        }
    }
}
