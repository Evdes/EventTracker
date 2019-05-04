using EventTracker.Models.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventTracker.DAL.Mocks
{
    public class EventInMemoryMock
    {
        private List<Event> _events;

        public EventInMemoryMock()
        {
            Populate();
        }

        private void Populate()
        {
            _events = new List<Event> {
                new Event { Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    WantedAmountOfParticipants = 1,
                    Timeframes = new List<Timeframe>
                    {
                        new Timeframe { EventDate = new DateTime(2021, 1, 1), Starttime = 11, Endtime = 21 },
                        new Timeframe { EventDate = new DateTime(2021, 1, 2), Starttime = 11, Endtime = 21 }
                    },
                    Location = new Location {City="City1", Province="Province1"},
                    IsCancelled=true
                },
                new Event { Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    WantedAmountOfParticipants = 2,
                    Timeframes = new List<Timeframe> {
                        new Timeframe { EventDate = new DateTime(2022, 2, 1), Starttime = 12, Endtime = 22 },
                        new Timeframe { EventDate = new DateTime(2022, 2, 2), Starttime = 12, Endtime = 22 }
                    },
                    Location = new Location {City="City2", Province="Province2"},
                },
                new Event { Id = 3,
                    Name = "Event3",
                    Description = "Description3",
                    WantedAmountOfParticipants = 3,
                    Timeframes = new List<Timeframe>
                        { new Timeframe { EventDate = new DateTime(2023, 2, 1), Starttime = 12, Endtime = 22}
                    },
                    Location = new Location {City="City3", Province="Province3"},
                }
            };
        }
        public List<Event> GetEvents()
        {
            return _events;
        }
    }
}
