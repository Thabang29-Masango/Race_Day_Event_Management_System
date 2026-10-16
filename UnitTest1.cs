using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class RaceDayControllerTests
    {
        private RaceDayContext GetDatabaseContext(bool seed = true)
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new RaceDayContext(options);
            databaseContext.Database.EnsureCreated();

            if (seed && !databaseContext.Events.Any())
            {
                for (int i = 1; i <= 5; i++)
                {
                    databaseContext.Events.Add(new Event
                    {
                        Name = "Event " + i,
                        Description = "Description for Event " + i,
                        EventDate = DateTime.Now.AddDays(i),
                        Location = "Location " + i,
                        DistanceKm = 5 + i,
                        EventType = "Type " + i,
                        OrganiserId = 1
                    });
                }
                databaseContext.SaveChanges();
            }

            return databaseContext;
        }

        [Fact]
        public async Task GetAll_ReturnsEvents()
        {
            using var db = GetDatabaseContext();
            var controller = new EventsController(db);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Event>>(ok.Value);
            Assert.NotEmpty(items);
        }

        [Fact]
        public async Task Create_WithValidOrganiser_CreatesEvent()
        {
            using var db = GetDatabaseContext(seed: false);

            // add organiser so the create check passes
            db.Organisers.Add(new Organiser
            {
                UserId = 1,
                FirstName = "A",
                LastName = "B",
                PhoneNumber = "123",
                OrganisationName = "Org"
            });
            db.SaveChanges();

            var controller = new EventsController(db);

            var newEvent = new Event
            {
                OrganiserId = 1,
                Name = "New Event",
                EventDate = DateTime.Now.AddDays(1),
                Location = "Here",
                DistanceKm = 10,
                EventType = "Run"
            };

            var result = await controller.Create(newEvent);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var createdEvent = Assert.IsType<Event>(created.Value);
            Assert.True(createdEvent.EventId > 0);
        }

        [Fact]
        public async Task Create_WithMissingOrganiser_ReturnsBadRequest()
        {
            using var db = GetDatabaseContext(seed: false);
            var controller = new EventsController(db);

            var newEvent = new Event
            {
                OrganiserId = 999,
                Name = "New Event",
                EventDate = DateTime.Now.AddDays(1),
                Location = "Here",
                DistanceKm = 10,
                EventType = "Run"
            };

            var result = await controller.Create(newEvent);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
