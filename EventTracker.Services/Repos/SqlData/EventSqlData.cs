using EventTracker.DAL.SqlData;
using EventTracker.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTracker.Services.Repos.SqlData
{
    public class EventSqlData : IEventRepo
    {
        private readonly EventTrackerDbContext _context;

        public EventSqlData(EventTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Event> AddEventAsync(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public async void DeleteEventAsync(Event eventToDelete)
        {
            _context.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<Event> EditEventAsync(Event eventToUpdate)
        {
            _context.Update(eventToUpdate);
            await _context.SaveChangesAsync();
            return eventToUpdate;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            return _context.Events
                .Include(e => e.Location)
                .Include(e => e.UserEvents).ThenInclude(ue => ue.UserProfile)
                .Include(e => e.Timeframes)
                .Where(e => e.Timeframes.Max(t => t.EventDate) >= DateTime.Today)
                .OrderBy(e => e.Timeframes.Min(t => t.EventDate));
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
