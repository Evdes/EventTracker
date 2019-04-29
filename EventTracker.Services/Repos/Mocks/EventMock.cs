using System;
using System.Collections.Generic;
using System.Linq;
using EventTracker.Models.Events;
using EventTracker.DAL.Mocks;

namespace EventTracker.Services.Repos.Mocks
{
    public class EventMock : IEventRepo
    {
        private readonly List<Event> _events;

        public EventMock(EventInMemoryMock eventMock)
        {
            _events = eventMock.GetEvents();
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

        public Event EditEvent(Event eventToUpdate)
        {
            return eventToUpdate;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            return _events.Where(e => e.Timeframes.Max(t => t.EventDate) >= DateTime.Today);
        }

        public Event GetEvent(int? id)
        {
            return _events.FirstOrDefault(e => e.Id == id.Value);
        }
    }
}
