using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDayManagementSystem.Controllers;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.DTOs;
using RaceDayManagementSystem.Models;
using Xunit;

namespace RaceDayAPI1.Tests
{
    public class AuthControllerTests
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

        private IConfiguration GetConfig()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string?>
            {
                ["Jwt:Key"] = "abcdefghijklmnopqrstuvwxyz012345", // >=32 chars
                ["Jwt:Issuer"] = "test",
                ["Jwt:Audience"] = "test"
            };
            return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        }

        [Fact]
        public async System.Threading.Tasks.Task Register_Organiser_Succeeds()
        {
            using var db = GetDb();
            var controller = new AuthController(db, GetConfig());

            var req = new RegisterRequest
            {
                Email = "org@example.com",
                Password = "Password123!",
                Role = "Organiser",
                FirstName = "Org",
                LastName = "Owner",
                PhoneNumber = "123",
                OrganisationName = "OrgName"
            };

            var res = await controller.Register(req);
            var created = Assert.IsType<ObjectResult>(res);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task Login_ReturnsToken()
        {
            using var db = GetDb();
            var cfg = GetConfig();
            var controller = new AuthController(db, cfg);

            var reg = new RegisterRequest
            {
                Email = "u1@example.com",
                Password = "Password123!",
                Role = "Participant",
                FirstName = "P",
                LastName = "L",
                DateOfBirth = System.DateTime.UtcNow.AddYears(-20)
            };

            var r1 = await controller.Register(reg);
            Assert.IsType<ObjectResult>(r1);

            var login = new LoginRequest { Email = "u1@example.com", Password = "Password123!" };
            var res = await controller.Login(login);
            var ok = Assert.IsType<OkObjectResult>(res);
            Assert.NotNull(ok.Value);
            var prop = ok.Value.GetType().GetProperty("token");
            Assert.NotNull(prop);
            var token = prop.GetValue(ok.Value) as string;
            Assert.False(string.IsNullOrEmpty(token));
        }
    }
}
