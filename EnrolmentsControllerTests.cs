using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class EnrolmentsControllerTests
    {
        private RaceDayContext GetDb()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var db = new RaceDayContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Valid_CreatesEnrolment()
        {
            using var db = GetDb();
            db.Participants.Add(new Participant { UserId = 50, FirstName = "A", LastName = "B", DateOfBirth = System.DateTime.UtcNow.AddYears(-30) });
            db.Events.Add(new Event { EventId = 201, OrganiserId = 1, Name = "Evt", EventDate = System.DateTime.UtcNow.AddDays(5), Location = "L", DistanceKm = 5, EventType = "T" });
            db.Categories.Add(new Category { CategoryId = 400, EventId = 201, Name = "C", MinAge = 0, MaxAge = 99, DistanceKm = 5 });
            db.SaveChanges();

            var controller = new EnrolmentsController(db);
            var input = new Enrolment { ParticipantId = 50, EventId = 201, CategoryId = 400 };
            var res = await controller.Create(input);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var en = Assert.IsType<Enrolment>(created.Value);
            Assert.True(en.EnrolmentId > 0);
            Assert.Equal("Active", en.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Duplicate_ReturnsConflict()
        {
            using var db = GetDb();
            db.Participants.Add(new Participant { UserId = 51, FirstName = "A", LastName = "B", DateOfBirth = System.DateTime.UtcNow.AddYears(-30) });
            db.Events.Add(new Event { EventId = 202, OrganiserId = 1, Name = "Evt2", EventDate = System.DateTime.UtcNow.AddDays(5), Location = "L", DistanceKm = 5, EventType = "T" });
            db.Categories.Add(new Category { CategoryId = 401, EventId = 202, Name = "C2", MinAge = 0, MaxAge = 99, DistanceKm = 5 });
            db.Enrolments.Add(new Enrolment { EnrolmentId = 900, ParticipantId = 51, EventId = 202, CategoryId = 401, Status = "Active", EnrolmentDate = System.DateTime.UtcNow });
            db.SaveChanges();

            var controller = new EnrolmentsController(db);
            var input = new Enrolment { ParticipantId = 51, EventId = 202, CategoryId = 401 };
            var res = await controller.Create(input);

            Assert.IsType<ConflictObjectResult>(res);
        }

        [Fact]
        public async System.Threading.Tasks.Task Cancel_SetsStatusCancelled()
        {
            using var db = GetDb();
            db.Enrolments.Add(new Enrolment { EnrolmentId = 910, ParticipantId = 60, EventId = 300, CategoryId = 500, Status = "Active", EnrolmentDate = System.DateTime.UtcNow });
            db.SaveChanges();

            var controller = new EnrolmentsController(db);
            var res = await controller.Cancel(910);

            var ok = Assert.IsType<OkObjectResult>(res);
            var en = Assert.IsType<Enrolment>(ok.Value);
            Assert.Equal("Cancelled", en.Status);
        }
    }
}
