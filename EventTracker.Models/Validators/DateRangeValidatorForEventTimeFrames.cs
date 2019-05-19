using System;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models
{
    public class DateIsNotInPast : RangeAttribute
    {
        public DateIsNotInPast() 
            : base(typeof(DateTime), 
                  DateTime.Today.ToShortDateString(), 
                  DateTime.MaxValue.ToShortDateString())
        {
            ErrorMessage = "Date cannot be situated in the past";
        }
    }
}