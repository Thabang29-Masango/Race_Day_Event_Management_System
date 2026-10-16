using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class ResultsControllerTests
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
        public async System.Threading.Tasks.Task Create_Valid_CreatesResult()
        {
            using var db = GetDb();
            db.Enrolments.Add(new Enrolment { EnrolmentId = 700, ParticipantId = 80, EventId = 400, CategoryId = 600, Status = "Active", EnrolmentDate = System.DateTime.UtcNow });
            db.SaveChanges();

            var controller = new ResultsController(db);
            var input = new Result { EnrolmentId = 700, FinishPosition = 1, TotalFinishers = 10, FinishTime = System.TimeSpan.FromMinutes(30) };
            var res = await controller.Create(input);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var r = Assert.IsType<Result>(created.Value);
            Assert.True(r.ResultId > 0);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Duplicate_ReturnsConflict()
        {
            using var db = GetDb();
            db.Enrolments.Add(new Enrolment { EnrolmentId = 701, ParticipantId = 81, EventId = 401, CategoryId = 601, Status = "Active", EnrolmentDate = System.DateTime.UtcNow });
            db.Results.Add(new Result { ResultId = 800, EnrolmentId = 701, FinishPosition = 2, TotalFinishers = 10 });
            db.SaveChanges();

            var controller = new ResultsController(db);
            var input = new Result { EnrolmentId = 701, FinishPosition = 1, TotalFinishers = 9 };
            var res = await controller.Create(input);

            Assert.IsType<ConflictObjectResult>(res);
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_RemovesResult()
        {
            using var db = GetDb();
            db.Results.Add(new Result { ResultId = 810, EnrolmentId = 720, FinishPosition = 3, TotalFinishers = 50 });
            db.SaveChanges();

            var controller = new ResultsController(db);
            var res = await controller.Delete(810);
            Assert.IsType<NoContentResult>(res);
            Assert.Null(await db.Results.FindAsync(810));
        }
    }
}
