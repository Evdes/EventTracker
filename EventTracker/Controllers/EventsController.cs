using EventTracker.Models;
using EventTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventRepo _events;

        public EventsController(IEventRepo eventMockRepo)
        {
            _events = eventMockRepo;
        }

        public IActionResult AllUpcomingEvents()
        {
            var allUpcomingEvents = _events.GetAllUpcomingEvents();
            return View(allUpcomingEvents);
        }

        public IActionResult EventDetails(int id)
        {
            var requestedEvent = _events.GetEvent(id);
            return View(requestedEvent);
        }

        [HttpGet]
        public IActionResult AddEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvent(Event newEvent)
        {
            newEvent = _events.AddEvent(newEvent);
            return RedirectToAction(nameof(EventDetails), new { id = newEvent.Id });
        }

        [HttpGet]
        public IActionResult DeleteEvent(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var eventToDelete = _events.GetEvent(id.Value);
                return View(eventToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEvent(Event eventToDelete)
        {
            _events.DeleteEvent(_events.GetEvent(eventToDelete.Id));
            return RedirectToAction(nameof(AllUpcomingEvents));
        }

        [HttpGet]
        public IActionResult EditEvent(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var eventToEdit = _events.GetEvent(id.Value);
                return View(eventToEdit);
            }
        }

        [HttpPost]
        public IActionResult EditEvent(Event updatedEvent)
        {
            updatedEvent = _events.EditEvent(updatedEvent);
            return RedirectToAction(nameof(EventDetails), new { id = updatedEvent.Id });
        }
    }
}