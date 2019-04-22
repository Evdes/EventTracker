using EventTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.Services
{
    public interface IEventRepo
    {
        IEnumerable<Event> GetAllUpcomingEvents();
        Event GetEvent(int id);
        Event AddEvent(Event newEvent);
        void DeleteEvent(Event eventToDelete);
        Event EditEvent(Event UpdatedEvent);
    }
}
