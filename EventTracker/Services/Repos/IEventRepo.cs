using EventTracker.BLL.Models.Events;
using System.Collections.Generic;

namespace EventTracker.BLL.Services.Repos
{
    public interface IEventRepo
    {
        IEnumerable<Event> GetAllUpcomingEvents();
        Event GetEvent(int? id);
        Event AddEvent(Event newEvent);
        void DeleteEvent(Event eventToDelete);
        Event EditEvent(Event postedEvent, Event eventToUpdate);
    }
}
