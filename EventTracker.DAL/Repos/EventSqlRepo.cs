using EventTracker.DAL.SqlData;
using EventTracker.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.DAL.Repos
{
    public class EventSqlRepo : IEventRepo
    {
        private readonly EventTrackerDbContext _context;

        public EventSqlRepo(EventTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Event> AddEventAsync(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public void DeleteEvent(Event eventToDelete)
        {
            _context.Remove(eventToDelete);
            _context.SaveChanges();
        }

        public async Task<Event> EditEventAsync(Event eventToUpdate)
        {
            _context.Update(eventToUpdate);
            await _context.SaveChangesAsync();
            return eventToUpdate;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            var allUpcomingEvents = _context.Events
           .Include(e => e.Location)
           .Include(e => e.UserEvents).ThenInclude(ue => ue.UserProfile)
           .Include(e => e.Timeframes)
           .Where(e => e.Timeframes.Max(t => t.EventDate) >= DateTime.Today)
           .OrderBy(e => e.Timeframes.Min(t => t.EventDate));
            return allUpcomingEvents;
        }

        public async Task<Event> GetEventAsync(int? id)
        {
            var @event = await _context.Events
                   .Include(e => e.Location)
                   .Include(e => e.UserEvents).ThenInclude(ue => ue.UserProfile)
                   .Include(e => e.Timeframes)
                   .FirstOrDefaultAsync(e => e.Id == id.Value);
            return @event;
        }
    }
}
