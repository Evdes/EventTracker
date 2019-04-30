using EventTracker.Models.Events;
using EventTracker.Services.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EventTracker.BLL.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventRepo _events;

        public EventsController(IEventRepo eventMockRepo)
        {
            _events = eventMockRepo;
        }

        [HttpGet]
        public IActionResult AllUpcomingEvents()
        {
            var allUpcomingEvents = _events.GetAllUpcomingEvents();
            return View(allUpcomingEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllUpcomingEvents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                ToggleCancel(id.Value);
                return RedirectToAction(nameof(AllUpcomingEvents));
            }
        }

        [HttpGet]
        public IActionResult EventDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var requestedEvent = _events.GetEvent(id.Value);
                return View(requestedEvent);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EventDetails")]
        public IActionResult EventDetailsPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                ToggleCancel(id.Value);
                var requestedEvent = _events.GetEvent(id.Value);
                return RedirectToAction(nameof(EventDetails), new { id = requestedEvent.Id });
            }
        }


        [HttpGet]
        public IActionResult AddEvent()
        {
            var eventToAdd = new Event();
            eventToAdd.Timeframes.Add(new Timeframe { EventDate = DateTime.Today, Starttime = 10, Endtime = 17 });
            return View(eventToAdd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvent(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                newEvent.IsCancelled = false;
                newEvent = _events.AddEvent(newEvent);
                return RedirectToAction(nameof(EventDetails), new { id = newEvent.Id });
            }
            else
            {
                return View(newEvent);
            }
            
        }

        [HttpGet]
        public IActionResult DeleteEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var eventToDelete = _events.GetEvent(id.Value);
                if (eventToDelete.IsCancelled)
                {
                    return RedirectToAction(nameof(EventDetails), new { id = id.Value });
                }
                else
                {
                    return View(eventToDelete);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteEvent")]
        public IActionResult DeleteEventPost(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                _events.DeleteEvent(_events.GetEvent(id.Value));
                return RedirectToAction(nameof(AllUpcomingEvents));
            }
        }

        [HttpGet]
        public IActionResult EditEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var eventToEdit = _events.GetEvent(id.Value);
                if (eventToEdit.IsCancelled)
                {
                    return RedirectToAction(nameof(EventDetails), new { id = eventToEdit.Id });
                }
                else
                {
                    return View(eventToEdit);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEvent(Event postedEvent)
        {
            if (ModelState.IsValid)
            {
                if (postedEvent == null)
                {
                    return NotFound();
                }
                else
                {
                    var eventToUpdate = _events.GetEvent(postedEvent.Id);

                    eventToUpdate.Name = postedEvent.Name;
                    eventToUpdate.Description = postedEvent.Description;
                    eventToUpdate.WantedAmountOfParticipants = postedEvent.WantedAmountOfParticipants;
                    eventToUpdate.Location.City = postedEvent.Location.City;
                    eventToUpdate.Location.Province = postedEvent.Location.Province;
                    eventToUpdate.Timeframes.Clear();

                    foreach (var timeframe in postedEvent.Timeframes)
                    {
                        eventToUpdate.Timeframes.Add(
                            new Timeframe
                            {
                                EventDate = timeframe.EventDate,
                                Starttime = timeframe.Starttime,
                                Endtime = timeframe.Endtime
                            });
                    }

                    eventToUpdate = _events.EditEvent(eventToUpdate);
                    return RedirectToAction(nameof(EventDetails), new { id = eventToUpdate.Id });
                }
            }
            else
            {
                return View(postedEvent);
            }
        }

        private void ToggleCancel(int id)
        {
            var eventToToggle = _events.GetEvent(id);
            eventToToggle.IsCancelled = !eventToToggle.IsCancelled;
            _events.EditEvent(eventToToggle);
        }
    }
}