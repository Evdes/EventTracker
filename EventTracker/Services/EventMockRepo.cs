using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTracker.BLL.Models;
using EventTracker.Models;


namespace EventTracker.Services
{
    public class EventMockRepo : IEventRepo
    {
        private List<Event> _events;

        public EventMockRepo()
        {
            PopulateMock();
        }

        private void PopulateMock()
        {
            _events = new List<Event> {
                new Event { Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    WantedAmountOfParticipants = 1,
                    Timeframes = new List<TimeFrame>
                    {
                        new TimeFrame { EventDate = new DateTime(2001, 1, 1), Starttime = 11, Endtime = 21 },
                        new TimeFrame { EventDate = new DateTime(2001, 1, 2), Starttime = 11, Endtime = 21 }
                    },
                    Location = new Location {City="City1", Province="Province1"},
                    Participants = new List<Participant>
                    {
                        new Participant { Name = "Participant1" }
                    }

                },
                new Event { Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    WantedAmountOfParticipants = 2,
                    Timeframes = new List<TimeFrame> {
                        new TimeFrame { EventDate = new DateTime(2002, 2, 1), Starttime = 12, Endtime = 22 },
                        new TimeFrame { EventDate = new DateTime(2002, 2, 2), Starttime = 12, Endtime = 22 }
                    },
                    Location = new Location {City="City2", Province="Province2"},
                    Participants = new List<Participant>
                    {
                        new Participant { Name = "Participant1" },
                        new Participant { Name = "Participant2" }
                    }
                },
                new Event { Id = 3,
                    Name = "Event3",
                    Description = "Description3",
                    WantedAmountOfParticipants = 3,
                    Timeframes = new List<TimeFrame>
                        { new TimeFrame { EventDate = new DateTime(2002, 2, 1), Starttime = 12, Endtime = 22}
                    },
                    Location = new Location {City="City3", Province="Province3"},
                    Participants = new List<Participant>
                    {
                        new Participant { Name = "Participant1" },
                        new Participant { Name = "Participant2" },
                        new Participant { Name = "Participant3" }
                    }
                }
            };
        }

        public Event AddEvent(Event newEvent)
        {
            newEvent.Id = _events.Max(e => e.Id) + 1;
            _events.Add(newEvent);
            return newEvent;
        }

        public void DeleteEvent(Event eventToDelete)
        {
            _events.Remove(eventToDelete);
        }

        public Event EditEvent(Event updatedEvent)
        {
            var eventToEdit = GetEvent(updatedEvent.Id);
            eventToEdit.Name = updatedEvent.Name;
            eventToEdit.Description = updatedEvent.Description;
            eventToEdit.WantedAmountOfParticipants = updatedEvent.WantedAmountOfParticipants;
            eventToEdit.Location.City = updatedEvent.Location.City;
            eventToEdit.Location.Province = updatedEvent.Location.Province;
            eventToEdit.Timeframes.Clear();
            foreach(var timeframe in updatedEvent.Timeframes)
            {
                eventToEdit.Timeframes.Add(
                    new TimeFrame
                    {
                        EventDate = timeframe.EventDate,
                        Starttime = timeframe.Starttime,
                        Endtime = timeframe.Endtime
                    });
            }
            return eventToEdit;
        }

        public IEnumerable<Event> GetAllUpcomingEvents()
        {
            return _events;
        }

        public Event GetEvent(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
    }
}
