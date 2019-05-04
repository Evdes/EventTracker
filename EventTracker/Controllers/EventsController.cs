using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.BLL.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventRepo _events;
        private readonly UserManager<UserProfile> _userManager;

        public EventsController(IEventRepo eventMockRepo, 
            UserManager<UserProfile> userManager)
        {
            _events = eventMockRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AllUpcomingEvents()
        {
            var allUpcomingEvents = _events.GetAllUpcomingEvents();
            return View(allUpcomingEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(AllUpcomingEvents))]
        public async Task<IActionResult> ToggleSubscribeFromAllAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                await ToggleSubscribeAsync(id);
                return RedirectToAction(nameof(AllUpcomingEvents), new { id = string.Empty});
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
        [ActionName(nameof(EventDetails))]
        public async Task<IActionResult> ToggleSubscribeFromDetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                await ToggleSubscribeAsync(id);
                return RedirectToAction(nameof(EventDetails), new { id = id.Value});
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super")]
        public IActionResult CancelEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                return View(_events.GetEvent(id.Value));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        [ActionName(nameof(CancelEvent))]
        public IActionResult CancelEventPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
                
            }
            else
            {
                var eventToCancel = _events.GetEvent(id.Value);
                ToggleCancel(eventToCancel);
                return RedirectToAction(nameof(EventDetails), new { id = eventToCancel.Id });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Super")]
        public IActionResult AddEvent()
        {
            var eventToAdd = new Event();
            eventToAdd.Timeframes.Add(new Timeframe { EventDate = DateTime.Today, Starttime = 10, Endtime = 17 });
            return View(eventToAdd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
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
        [Authorize(Roles = "Super")]
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
        [Authorize(Roles = "Super")]
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
                return RedirectToAction(nameof(AllUpcomingEvents), new { id = string.Empty });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super")]
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
        [Authorize(Roles = "Super")]
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

        private void ToggleCancel(Event @event)
        {
            @event.IsCancelled = !@event.IsCancelled;
            _events.EditEvent(@event);
        }

        private async Task ToggleSubscribeAsync(int? id)
        {
            var @event = _events.GetEvent(id.Value);
            var userProfile = await _userManager.GetUserAsync(HttpContext.User);
            var subscription = new UserEvents
            {
                Event = @event,
                EventId = @event.Id,
                UserProfile = userProfile,
                UserId = userProfile.Id
            };

            if (@event.UserEvents.Any(ue => ue.UserId == userProfile.Id))
            {
                var ueToRemove = @event.UserEvents.FirstOrDefault(ue => ue.UserId == userProfile.Id);
                @event.UserEvents.Remove(ueToRemove);
            }
            else
            {
                @event.UserEvents.Add(subscription);
            }
            _events.EditEvent(@event);
        }
    }
}