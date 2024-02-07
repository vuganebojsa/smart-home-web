using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class AmbientSensorDataDTO
    {
        public string DeviceId { get; set; }
        public double RoomTemperature { get; set; }
        public double RoomHumidity { get; set; }
    }
}
