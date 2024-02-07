using SmartHouse.Core.Messages;
using SmartHouse.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.DTOS
{
    public class RegisterPropertyDTO
    {
        [Required(ErrorMessage = "Type is required.")]
        public TypeOfProperty TypeOfProperty { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name should be at least 3 characters long")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; } = string.Empty;
        [Required(ErrorMessage = "Quadrature is required.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Minimum Quadrature is 1.") ]
        public double Quadrature { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Minimum number of floors is 1.")]
        [Required(ErrorMessage = "Number of floors is required.")]
        public int NumberOfFloors { get; set; }
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Image type is required")]
        public string ImageType { get; set; } = "jpg";

    }
}
