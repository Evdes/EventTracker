using EventTracker.Models.Events;
using System.Collections.Generic;

namespace EventTracker.Services.Repos
{
    public interface IEventRepo
    {
        IEnumerable<Event> GetAllUpcomingEvents();
        Event GetEvent(int? id);
        Event AddEvent(Event newEvent);
        void DeleteEvent(Event eventToDelete);
        Event EditEvent(Event eventToUpdate);
    }
}
