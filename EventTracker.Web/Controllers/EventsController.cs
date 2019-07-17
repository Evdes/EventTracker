using EventTracker.DAL.Repos;
using EventTracker.Models.Events;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.Alerts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventTracker.Web.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventRepo _events;
        private readonly UserManager<UserProfile> _userManager;

        public EventsController(IEventRepo eventRepo,
            UserManager<UserProfile> userManager)
        {
            _events = eventRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult UpcomingEvents()
        {
            return View(_events.GetAllUpcomingEvents());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSubscribeAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
            }
            else
            {
                var @event = await _events.GetEventAsync(id.Value);
                if (@event == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
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
                    await _events.EditEventAsync(@event);
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                        .WithSuccess("Success", "You have altered your presence for this event");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> CancelEventAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
            }
            else
            {
                var @event = await _events.GetEventAsync(id.Value);
                if (@event == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
                    @event.IsCancelled = !@event.IsCancelled;
                    await _events.EditEventAsync(@event);
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
        public async Task<IActionResult> AddEventAsync(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                newEvent.IsCancelled = false;
                await _events.AddEventAsync(newEvent);
                return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                    .WithSuccess("Success", "Event added");
            }
            else
            {
                return View(newEvent).WithDanger("Failed", "Event not added");
            }
        }

        [HttpGet]
        public ActionResult TimeFrameEntry()
        {
            return PartialView("Partials/_Timeframes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> DeleteEventAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
            }
            else
            {
                var eventToDelete = await _events.GetEventAsync(id.Value);
                if (eventToDelete == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
                    _events.DeleteEvent(eventToDelete);
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty }).WithSuccess("Success", "Event deleted");
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super")]
        [ActionName("EditEvent")]
        public async Task<IActionResult> EditEventAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
            }
            else
            {
                var eventToEdit = await _events.GetEventAsync(id.Value);
                if (eventToEdit == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
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
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        [ActionName("EditEvent")]
        public async Task<IActionResult> EditEventAsync(Event postedEvent)
        {
            if (ModelState.IsValid)
            {
                var eventToUpdate = await _events.GetEventAsync(postedEvent.Id);
                if (eventToUpdate == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
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

                    await _events.EditEventAsync(eventToUpdate);
                    return RedirectToAction(nameof(UpcomingEvents), new { id = string.Empty })
                    .WithSuccess("Success", "Event updated");
                }
            }
            else
            {
                return View(postedEvent).WithDanger("Failed", "Event not updated");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> RemoveParticipantAsync(int? id, string userId, Event postedEvent)
        {
            if (ModelState.IsValid)
            {
                var eventToModify = await _events.GetEventAsync(id);
                if(eventToModify == null)
                {
                    return RedirectToAction(nameof(ErrorController.EventNotFound), "error");
                }
                else
                {
                    var userProfileToRemove = await _userManager.FindByIdAsync(userId);
                    var ueToRemove = eventToModify.UserEvents.FirstOrDefault(ue => ue.UserId == userProfileToRemove.Id);
                    eventToModify.UserEvents.Remove(ueToRemove);
                    await EditEventAsync(postedEvent);
                    return RedirectToAction("EditEvent", new { id }).WithSuccess("Success", "Participant removed");
                }
                
            }
            else
            {
                return View("EditEvent", new { id });
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
    }
}