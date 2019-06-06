using System;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models
{
    public class DateNotInPastAttribute : RangeAttribute
    {
        public DateNotInPastAttribute() 
            : base(typeof(DateTime), 
                  DateTime.Today.ToShortDateString(), 
                  DateTime.MaxValue.ToShortDateString())
        {
            ErrorMessage = "Date cannot be situated in the past";
        }
    }
}