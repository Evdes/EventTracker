using System;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.Events
{
    public class Timeframe
    {
        public int Id { get; set; }

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
