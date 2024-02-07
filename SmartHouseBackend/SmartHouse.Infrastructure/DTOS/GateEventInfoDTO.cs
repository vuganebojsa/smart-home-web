using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class GateEventInfoDTO
    {
        public String Timestamp { get; set; }

        public String licencePlate { get; set; }
        public int? Action { get; set; }
    }
}
