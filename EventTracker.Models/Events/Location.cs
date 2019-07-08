using System.ComponentModel.DataAnnotations;

namespace EventTracker.Models.Events
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage ="The city you've entered contains too many characters")]
        public string City { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The city you've entered contains too many characters")]
        public string Province { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
