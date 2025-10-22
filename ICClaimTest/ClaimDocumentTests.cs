using Xunit;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using CMCS.Controllers;
using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ICClaimTest
{
    public class ClaimDocumentTests
    {
        // Helper: In-memory EF Core database
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + System.Guid.NewGuid())
                .Options;
            return new ApplicationDbContext(options);
        }

        // Helper: Mock IWebHostEnvironment
        private IWebHostEnvironment GetMockEnvironment()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(m => m.WebRootPath).Returns("wwwroot");
            return mockEnv.Object;
        }

        // Helper: Mock ILogger<T>
        private ILogger<T> GetMockLogger<T>()
        {
            var mockLogger = new Mock<ILogger<T>>();
            return mockLogger.Object;
        }

        // --------------------- LecturerController Tests ---------------------
        [Fact]
        public async Task LecturerController_CanCreateClaimDocument()
        {
            var context = GetDbContext();
            var env = GetMockEnvironment();
            var controller = new LecturerController(context, env);

            var claim = new ClaimDocument
            {
                MentorName = "John Doe",
                HoursWorked = 10,
                HourlyRate = 25,
                Status = "Pending",
                PaymentStatus = "Unpaid"
            };

            context.ClaimDocuments.Add(claim);
            await context.SaveChangesAsync();

            Assert.Single(context.ClaimDocuments);
            Assert.Equal("John Doe", context.ClaimDocuments.First().MentorName);
        }
        [Fact]
        public async Task LecturerController_CanRetrieveMyLogs()
        {
            var context = GetDbContext();
            var env = GetMockEnvironment();
            var controller = new LecturerController(context, env);

            context.ClaimDocuments.Add(new ClaimDocument { MentorName = "Alice", Status = "Pending" });
            context.ClaimDocuments.Add(new ClaimDocument { MentorName = "Alice", Status = "Approved" });
            await context.SaveChangesAsync();

            // 1. Use pattern matching to assert the result is a ViewResult 
            // and assign it to a non-nullable 'viewResult' variable.
            if (await controller.MyLogs("Alice", "Pending") is ViewResult viewResult)
            {
                // 2. Use pattern matching to assert the Model is IEnumerable<ClaimDocument>
                // and assign it to a non-nullable 'model' variable.
                if (viewResult.Model is IEnumerable<ClaimDocument> model)
                {
                    // The compiler now knows 'model' is not null inside this block.
                    Assert.Single(model);
                    Assert.Equal("Pending", model.First().Status);
                }
                else
                {
                    // Fail the test explicitly if the model was the wrong type or null
                    Assert.Fail("The result Model was null or not an IEnumerable<ClaimDocument>.");
                }
            }
            else
            {
                // Fail the test if the controller didn't return a ViewResult
                Assert.Fail("The controller did not return a ViewResult.");
            }
        }

        // --------------------- CoordinatorController Tests ---------------------
        [Fact]
        public async Task CoordinatorController_CanApproveClaim()
        {
            var context = GetDbContext();
            var controller = new CoordinatorController(context);

            var claim = new ClaimDocument
            {
                MentorName = "Jane",
                HoursWorked = 8,
                HourlyRate = 30,
                Status = "Pending",
                PaymentStatus = "Unpaid"
            };

            context.ClaimDocuments.Add(claim);
            await context.SaveChangesAsync();

            await controller.Approve(claim.Id);

            var updated = context.ClaimDocuments.First();
            Assert.Equal("CoordinatorApproved", updated.Status);
        }

        [Fact]
        public async Task CoordinatorController_CanRejectClaim_WithReason()
        {
            var context = GetDbContext();
            var controller = new CoordinatorController(context);

            var claim = new ClaimDocument
            {
                MentorName = "Bob",
                HoursWorked = 5,
                HourlyRate = 20,
                Status = "Pending",
                PaymentStatus = "Unpaid"
            };

            context.ClaimDocuments.Add(claim);
            await context.SaveChangesAsync();

            await controller.Reject(claim.Id, "Incorrect hours");

            var updated = context.ClaimDocuments.First();
            Assert.Equal("NeedsFix", updated.Status);
            Assert.Equal("Incorrect hours", updated.ReviewNote);
        }

        // --------------------- ManagerController Tests ---------------------
        [Fact]
        public async Task ManagerController_CanMarkAsPaid()
        {
            var context = GetDbContext();
            var env = GetMockEnvironment();
            var controller = new ManagerController(context);

            var claim = new ClaimDocument
            {
                MentorName = "Mark",
                HoursWorked = 5,
                HourlyRate = 40,
                Status = "CoordinatorApproved",
                PaymentStatus = "Unpaid"
            };

            context.ClaimDocuments.Add(claim);
            await context.SaveChangesAsync();

            claim.PaymentStatus = "Paid";
            claim.PaidAtUtc = System.DateTime.UtcNow;
            await context.SaveChangesAsync();

            var updated = context.ClaimDocuments.First();
            Assert.Equal("Paid", updated.PaymentStatus);
            Assert.NotNull(updated.PaidAtUtc);
        }

        // --------------------- HomeController Tests ---------------------
        [Fact]
        public void HomeController_Index_ReturnsView()
        {
            var mockLogger = GetMockLogger<HomeController>();
            var controller = new HomeController(mockLogger);

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Privacy_ReturnsView()
        {
            var mockLogger = GetMockLogger<HomeController>();
            var controller = new HomeController(mockLogger);

            var result = controller.Privacy() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}

