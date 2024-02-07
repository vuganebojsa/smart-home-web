using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.MqttDTOs
{
    public  class TemperatureDTO
    {
        public DateTime Timestamp { get; set; }
        public double RoomTemperature { get; set; }
    }
}
