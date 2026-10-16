using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class CategoriesControllerTests
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
        public async System.Threading.Tasks.Task GetByEvent_ReturnsCategories()
        {
            using var db = GetDb();
            db.Events.Add(new Event { EventId = 100, OrganiserId = 1, Name = "E", EventDate = System.DateTime.UtcNow.AddDays(1), Location = "L", DistanceKm = 10, EventType = "T" });
            db.Categories.Add(new Category { CategoryId = 200, EventId = 100, Name = "Cat", MinAge = 0, MaxAge = 99, DistanceKm = 10 });
            db.SaveChanges();

            var controller = new CategoriesController(db);
            var res = await controller.GetByEvent(100);

            var ok = Assert.IsType<OkObjectResult>(res);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Category>>(ok.Value);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_InvalidEvent_ReturnsNotFound()
        {
            using var db = GetDb();
            var controller = new CategoriesController(db);
            var input = new Category { Name = "X", MinAge = 0, MaxAge = 10, DistanceKm = 5 };
            var res = await controller.Create(999, input);
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Valid_CreatesCategory()
        {
            using var db = GetDb();
            db.Events.Add(new Event { EventId = 101, OrganiserId = 1, Name = "E2", EventDate = System.DateTime.UtcNow.AddDays(2), Location = "L", DistanceKm = 10, EventType = "T" });
            db.SaveChanges();

            var controller = new CategoriesController(db);
            var input = new Category { Name = "X", MinAge = 0, MaxAge = 10, DistanceKm = 5 };
            var res = await controller.Create(101, input);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var cat = Assert.IsType<Category>(created.Value);
            Assert.True(cat.CategoryId > 0);
            Assert.Equal(101, cat.EventId);
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_RemovesCategory()
        {
            using var db = GetDb();
            db.Categories.Add(new Category { CategoryId = 300, EventId = 1, Name = "ToDelete", MinAge = 0, MaxAge = 10, DistanceKm = 5 });
            db.SaveChanges();

            var controller = new CategoriesController(db);
            var res = await controller.Delete(300);
            Assert.IsType<NoContentResult>(res);
            Assert.Null(await db.Categories.FindAsync(300));
        }
    }
}
