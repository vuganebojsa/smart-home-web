using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SprinklerEventInfoDTO
    {
        public string Timestamp { get; set; }

        public string Username { get; set; }

        public string Value { get; set; }
        public string? Action { get; set; }
    }
}
