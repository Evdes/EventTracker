using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.Models
{
    //Minvalue = today, Maxvalue = 31/12/9999
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