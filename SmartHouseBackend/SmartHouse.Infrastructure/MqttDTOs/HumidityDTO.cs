using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.MqttDTOs
{
    public class HumidityDTO
    {
        public DateTime Timestamp { get; set; }
        public double RoomHumidity { get; set; }
    }
}
