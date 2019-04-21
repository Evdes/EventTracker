using EventTracker.Controllers;
using EventTracker.Models;
using EventTracker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;


namespace EventTracker.Tests
{
    [TestClass]
    public class EventManagementTests
    {
        private IEventRepo _events;
        private EventsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var eventMockRepo = new EventMockRepo();
            _events = eventMockRepo;

            var controller = new EventsController(_events);
            _controller = controller;
        }

        [TestMethod]
        public void GetAllEvents()
        {
            //Arrange
            var expected = _events.GetAllUpcomingEvents().ToList().Count;
            var actual = _controller.GetAllUpcomingEvents().ExecuteResultAsync;

            //Act

            //Assert
        }

        [TestMethod]
        public void GetCorrectEventDetails()
        {

        }


    }
}
