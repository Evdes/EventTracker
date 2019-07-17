using EventTracker.Models.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventTracker.DAL.Repos
{
    public interface IEventRepo
    {
        IEnumerable<Event> GetAllUpcomingEvents();
        Task<Event> GetEventAsync(int? id);
        Task<Event> AddEventAsync(Event newEvent);
        void DeleteEvent(Event eventToDelete);
        Task<Event> EditEventAsync(Event eventToUpdate);
    }
}
