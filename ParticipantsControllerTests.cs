using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class ParticipantsControllerTests
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
        public async System.Threading.Tasks.Task Get_ReturnsParticipant()
        {
            using var db = GetDb();
            db.Participants.Add(new Participant { UserId = 20, FirstName = "P", LastName = "Q", DateOfBirth = System.DateTime.UtcNow.AddYears(-30) });
            db.SaveChanges();

            var controller = new ParticipantsController(db);
            var res = await controller.Get(20);

            var ok = Assert.IsType<OkObjectResult>(res);
            var item = Assert.IsType<Participant>(ok.Value);
            Assert.Equal(20, item.UserId);
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Existing_ReturnsOk()
        {
            using var db = GetDb();
            db.Participants.Add(new Participant { UserId = 21, FirstName = "Old", LastName = "Name", DateOfBirth = System.DateTime.UtcNow.AddYears(-25) });
            db.SaveChanges();

            var controller = new ParticipantsController(db);
            var input = new Participant { UserId = 21, FirstName = "New", LastName = "Name2", DateOfBirth = System.DateTime.UtcNow.AddYears(-26), ProfilePictureUrl = "img.jpg" };
            var res = await controller.Update(21, input);

            var ok = Assert.IsType<OkObjectResult>(res);
            var updated = Assert.IsType<Participant>(ok.Value);
            Assert.Equal("New", updated.FirstName);
            Assert.Equal("img.jpg", updated.ProfilePictureUrl);
        }
    }
}
