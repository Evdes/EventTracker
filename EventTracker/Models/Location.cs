using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.BLL.Models
{
    public class Location
    {
        [Required]
        [MaxLength(50, ErrorMessage ="The city you've entered contains too many characters")]
        public string City { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "The city you've entered contains too many characters")]
        public string Province { get; set; }
    }
}
