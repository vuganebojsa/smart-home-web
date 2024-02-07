using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.MqttDTOs
{
    public class LampLuminosityDTO
    {
        public string DeviceId { get; set; }
        public double TotalLuminosity { get; set; }
    }
}
