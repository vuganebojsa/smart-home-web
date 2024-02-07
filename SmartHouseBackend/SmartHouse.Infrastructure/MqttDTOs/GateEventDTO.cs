using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.MqttDTOs
{
    public class GateEventDTO
    {
        public string DeviceId { get; set; }
        public string licencePlate { get; set; }
        public int action { get; set; }

    }
}
