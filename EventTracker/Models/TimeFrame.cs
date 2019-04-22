using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.BLL.Models
{
    public class TimeFrame
    {
        public DateTime EventDate { get; set; }
        public int Starttime { get; set; }
        public int Endtime { get; set; }
    }
}
