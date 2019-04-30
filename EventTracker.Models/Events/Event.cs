using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.Events
{
    public class Event
    {
        public Event()
        {
            Timeframes = new List<Timeframe>();
            Participants = new List<Participant>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name="Event Name")]
        [MaxLength(50, ErrorMessage ="The name you've entered is too long")]
        public string Name { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "The description you've entered is too long")]
        public string Description { get; set; }

        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="Enter a valid number larger than 0")]
        public int WantedAmountOfParticipants { get; set; }


        public List<Timeframe> Timeframes { get; set; }
        public Location Location { get; set; }
        public List<Participant> Participants { get; set; }
        public bool IsCancelled { get; set; }
    }
}
