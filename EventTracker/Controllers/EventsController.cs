using EventTracker.BLL.Extensions.Alerts;
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
        public IActionResult UpcomingEvents()
        {
            var allUpcomingEvents = _events.GetAllUpcomingEvents();
            return View(allUpcomingEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSubscribe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                await ToggleSubscribeAsync(id);
                return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                    .WithSuccess("Success", "You have altered your presence for this event");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public IActionResult CancelEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            else
            {
                var @event = _events.GetEvent(id.Value);
                ToggleCancel(@event);
                if (@event.IsCancelled)
                {
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty }).WithSuccess("Success", "Event was cancelled");
                }
                else
                {
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty }).WithSuccess("Success", "Event is no longer cancelled");
                }
                
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
                _events.AddEvent(newEvent);
                return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                    .WithSuccess("Success", "Event added");
            }
            else
            {
                return View(newEvent).WithDanger("Failed", "Event not added");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public IActionResult DeleteEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                _events.DeleteEvent(_events.GetEvent(id.Value));
                return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty }).WithSuccess("Success", "Event deleted");
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
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty });
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
                    _events.EditEvent(eventToUpdate);
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                    .WithSuccess("Success", "Event updated");
            }
            else
            {
                return View(postedEvent).WithDanger("Failed", "Event not updated");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> RemoveParticipantAsync(int? id, string userId)
        {
            if (ModelState.IsValid)
            {
                var eventToModify = _events.GetEvent(id);
                var userProfileToRemove = await _userManager.FindByIdAsync(userId);
                var ueToRemove = eventToModify.UserEvents.FirstOrDefault(ue => ue.UserId == userProfileToRemove.Id);
                eventToModify.UserEvents.Remove(ueToRemove);
                _events.EditEvent(eventToModify);
                return RedirectToAction(nameof(EditEvent), new { id }).WithSuccess("Success","Participant removed");
            }
            else
            {
                return View(nameof(EditEvent), new { id });
            }
        }

        [HttpGet]
        [ActionName("MyEvents")]
        public async Task<IActionResult> MyEventsAsync()
        {
            var allEvents = _events.GetAllUpcomingEvents();
            List<Event> MyEvents = new List<Event>();

            foreach (var @event in allEvents)
            {

                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (@event.UserEvents.Any(e => e.UserId == currentUser.Id))
                {
                    MyEvents.Add(@event);
                }
            }
            return View(MyEvents);
        }

        private void ToggleCancel(Event @event)
        {
            @event.IsCancelled = !@event.IsCancelled;
            _events.EditEvent(@event);
        }
        private async Task ToggleSubscribeAsync(int? id)
        {
            var @event = _events.GetEvent(id.Value);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var subscription = new UserEvents
            {
                Event = @event,
                EventId = @event.Id,
                UserProfile = currentUser,
                UserId = currentUser.Id
            };

            if (@event.UserEvents.Any(ue => ue.UserId == currentUser.Id))
            {
                var ueToRemove = @event.UserEvents.FirstOrDefault(ue => ue.UserId == currentUser.Id);
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