using SmartHouse.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class LicencePlateDTO
    {
        [Required(ErrorMessage = "Please enter licence plate.")]
        [MinLength(6, ErrorMessage = "Licence plate should be at least 6 characters long")]

        public string plate { get; set; }
    }
}
