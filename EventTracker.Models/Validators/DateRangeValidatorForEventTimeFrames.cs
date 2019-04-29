using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.Models
{
    //Minvalue = today, Maxvalue = 31/12/9999
    public class DateRangeValidatorForEventTimeFrames : RangeAttribute
    {
        public DateRangeValidatorForEventTimeFrames() 
            : base(typeof(DateTime), 
                  DateTime.Today.ToShortDateString(), 
                  DateTime.MaxValue.ToShortDateString())
        {
            ErrorMessage = "Date cannot be situated in the past";
        }
    }
}