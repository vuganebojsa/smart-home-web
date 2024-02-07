using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class LuminosityDataDTO
    {
        public String Timestamp { get; set; }
        public double? Luminosity { get; set; }
    }
}
