using EventTracker.Models.UserProfiles;

namespace EventTracker.Models.Events
{
    public class UserEvents
    {
        public string UserId { get; set; }
        public virtual UserProfile UserProfile { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }
    }
}
