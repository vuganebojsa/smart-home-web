using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SprinklerInfoDTO
    {
        public bool isPublic { get; set; }
        public bool isOn { get; set; }
        public bool isOnline { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

    }
}
