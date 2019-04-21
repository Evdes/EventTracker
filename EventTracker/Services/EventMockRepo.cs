using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.Models;


namespace EventTracker.Services
{
    public class EventMockRepo : IEventRepo
    {
        private readonly List<Event> _events;

        public EventMockRepo()
        {
            _events = new List<Event> {
                                new Event { Id = 1, Name = "Event1" },
                                new Event { Id = 2, Name = "Event2" },
                                new Event { Id = 3, Name = "Event3" }
            };
        }

        public Event AddEvent(Event newEvent)
        {
            newEvent.Id = _events.Max(e => e.Id) + 1;
            _events.Add(newEvent);
            return newEvent;
        }

        public void DeleteEvent(Event eventToDelete)
        {
            _events.Remove(eventToDelete);
        }

        public Event EditEvent(int eventToEditId, Event UpdatedEvent)
        {
            var eventToEdit = GetEvent(eventToEditId);
            eventToEdit.Name = UpdatedEvent.Name;
            return eventToEdit;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            return _events;
        }

        public Event GetEvent(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
    }
}
