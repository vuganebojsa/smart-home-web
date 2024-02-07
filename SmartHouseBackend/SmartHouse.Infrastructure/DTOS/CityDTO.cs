using SmartHouse.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class CityDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public CityDTO() {
        
        }
    }
}
