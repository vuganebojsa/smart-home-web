using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class ProcessPropertyRequestDTO
    {
        [Required(ErrorMessage = "Acceptance is required")]
        public bool Accept { get; set; }
        public string? Reason { get; set; }
    }
}
