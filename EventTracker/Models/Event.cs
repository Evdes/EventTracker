using EventTracker.BLL.Models;
using System.Collections.Generic;

namespace EventTracker.Models
{
    public class Event
    {
        public Event()
        {
            Timeframes = new List<TimeFrame>();
            Participants = new List<Participant>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WantedAmountOfParticipants { get; set; }
        public List<TimeFrame> Timeframes { get; set; }
        public Location Location { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
