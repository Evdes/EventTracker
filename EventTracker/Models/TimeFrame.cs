using EventTracker.BLL.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.BLL.Models
{
    public class TimeFrame
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage ="This date is invalid")]
        [DateRangeValidatorForEventTimeFrames]
        [Display(Name = "Event Date")]
        public DateTime? EventDate { get; set; }

        [Required]
        [Range(0,24, ErrorMessage ="Invalid hour")]
        [Display(Name = "Start")]
        public int? Starttime { get; set; }

        [Required]
        [Range(0, 24, ErrorMessage = "Invalid hour")]
        [Display(Name = "End")]
        public int? Endtime { get; set; }
    }
}
