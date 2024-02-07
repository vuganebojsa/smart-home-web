using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class GatePublicPrivateInfoDTO
    {
        public String Timestamp { get; set; }

        public int isPublic { get; set; }
    }
}
