using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Services;
using EventTracker.ViewModels;
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

        public IActionResult GetAllUpcomingEvents()
        {
            var allUpcomingEvents = _events.GetAllUpcomingEvents();
            return View("AllUpcomingEvents", allUpcomingEvents);
        }

        public IActionResult GetEventDetails(int id)
        {
            var requestedEvent = _events.GetEvent(id);
            return View("EventDetails", requestedEvent);
        }

        [HttpGet]
        public IActionResult AddEvent()
        {
            return View("AddEvent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvent(EventViewModel newEventViewModel)
        {
            var newEvent = new Event{Name = newEventViewModel.Name};
            newEvent = _events.AddEvent(newEvent);
            return RedirectToAction(nameof(GetEventDetails), new { id = newEvent.Id });
        }

        public IActionResult DeleteEvent(int id)
        {
            _events.DeleteEvent(_events.GetEvent(id));
            return RedirectToAction(nameof(GetAllUpcomingEvents));
        }

        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var eventToEdit = _events.GetEvent(id);
            var eventToEditViewModel = new EventViewModel
            {
                Name = eventToEdit.Name,
                Id = eventToEdit.Id
            };
            return View("EditEvent", eventToEditViewModel);
        }

        [HttpPost]
        public IActionResult EditEvent(EventViewModel updatedEventViewModel)
        {
            var eventToUpdate = new Event() { Name = updatedEventViewModel.Name };
            eventToUpdate = _events.EditEvent(updatedEventViewModel.Id, eventToUpdate);
            return RedirectToAction(nameof(GetEventDetails), new { id = eventToUpdate.Id });
        }
     }
}