using SmartHouse.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class DayDTO
    {
       
        [Required(ErrorMessage = "Please enter a day.")]
        public string day { get; set; }
        
    }
}
