using EventTracker.DAL.SqlData;
using EventTracker.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventTracker.Services.Repos.SqlData
{
    public class EventSqlData : IEventRepo
    {
        private readonly EventTrackerDbContext _context;

        public EventSqlData(EventTrackerDbContext context)
        {
            _context = context;
        }

        public Event AddEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return newEvent;
        }

        public void DeleteEvent(Event eventToDelete)
        {
            _context.Remove(eventToDelete);
            _context.SaveChanges();
        }

        public Event EditEvent(Event eventToUpdate)
        {
            _context.Update(eventToUpdate);
            _context.SaveChanges();
            return eventToUpdate;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            return _context.Events
                .Include(e => e.Location)
                .Include(e => e.UserEvents)
                .Include(e => e.Timeframes)
                .Where(e => e.Timeframes.Max(t => t.EventDate) >= DateTime.Today)
                .OrderBy(e => e.Timeframes.Min(t => t.EventDate));
        }

        public Event GetEvent(int? id)
        {
            return _context.Events
                .Include(e => e.Location)
                .Include(e => e.UserEvents).ThenInclude(ue => ue.UserProfile)
                .Include(e => e.Timeframes)
                .FirstOrDefault(e => e.Id == id.Value);
        }
    }
}
