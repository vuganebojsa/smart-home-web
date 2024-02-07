using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class OnlineOfflineHistoryDTO
    {
        
        public int TotalTimeOnline { get; set; }
        public int TotalTimeOffline { get; set;}

        public double PercentageOnline { get; set; }

        public Dictionary<string, int> OnlineMap { get; set; }

        public Dictionary<string, int> OfflineMap { get; set; }
    }
}
