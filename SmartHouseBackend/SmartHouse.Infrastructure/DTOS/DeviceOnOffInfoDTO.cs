using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class DeviceOnOffInfoDTO
    {
        public String Timestamp { get; set; }

        public int isOn { get; set; }
        
    }
}
