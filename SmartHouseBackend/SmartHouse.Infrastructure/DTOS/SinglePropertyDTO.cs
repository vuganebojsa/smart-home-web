using SmartHouse.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SinglePropertyDTO
    {
        public Guid id { get; set; }
        public TypeOfProperty TypeOfProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Activation IsAccepted { get; set; }
        public double Quadrature { get; set; }
        public int NumberOfFloors { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public ICollection<SmartDevice> Devices { get; set; }
    }
}
