using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class OrganisersControllerTests
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
        public async System.Threading.Tasks.Task Get_ReturnsOrganiser()
        {
            using var db = GetDb();
            db.Organisers.Add(new Organiser { UserId = 10, FirstName = "A", LastName = "B", PhoneNumber = "123", OrganisationName = "Org" });
            db.SaveChanges();

            var controller = new OrganisersController(db);
            var res = await controller.Get(10);

            var ok = Assert.IsType<OkObjectResult>(res);
            var item = Assert.IsType<Organiser>(ok.Value);
            Assert.Equal(10, item.UserId);
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Existing_ReturnsOk()
        {
            using var db = GetDb();
            db.Organisers.Add(new Organiser { UserId = 11, FirstName = "A", LastName = "B", PhoneNumber = "123", OrganisationName = "Org" });
            db.SaveChanges();

            var controller = new OrganisersController(db);
            var input = new Organiser { UserId = 11, FirstName = "X", LastName = "Y", PhoneNumber = "999", OrganisationName = "NewOrg" };
            var res = await controller.Update(11, input);

            var ok = Assert.IsType<OkObjectResult>(res);
            var updated = Assert.IsType<Organiser>(ok.Value);
            Assert.Equal("X", updated.FirstName);
            Assert.Equal("NewOrg", updated.OrganisationName);
        }
    }
}
