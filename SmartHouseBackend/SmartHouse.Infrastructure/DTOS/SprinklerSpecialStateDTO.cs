using SmartHouse.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SprinklerSpecialStateDTO
    {
        [Required(ErrorMessage = "You need to set the state which you want for the device.")]
        public bool IsActive { get; set; } = false;
    }
}
