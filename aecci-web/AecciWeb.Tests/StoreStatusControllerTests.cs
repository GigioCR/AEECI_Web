using Xunit;
using AecciWeb.Controllers;
using AecciWeb.Models;
using AecciWeb.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AecciWeb.Tests
{
    public class StoreStatusControllerTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetStatus_ReturnsLatestStatus()
        {
            // Arrange
            var context = GetDbContext(nameof(GetStatus_ReturnsLatestStatus));
            context.StoreStatus.Add(new StoreStatus { IsOpen = false });
            context.StoreStatus.Add(new StoreStatus { IsOpen = true });
            context.SaveChanges();

            var controller = new StoreStatusController(context);

            // Act
            var result = await controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<ActionResult<StoreStatusDto>>(result);
            var value = Assert.IsType<StoreStatusDto>(okResult.Value);
            Assert.True(value.IsOpen); // Should return the most recent, which is true
        }

        [Fact]
        public async Task GetStatus_ReturnsNotFound_IfNone()
        {
            // Arrange
            var context = GetDbContext(nameof(GetStatus_ReturnsNotFound_IfNone));
            var controller = new StoreStatusController(context);

            // Act
            var result = await controller.GetStatus();

            // Assert
            var notFound = Assert.IsType<ActionResult<StoreStatusDto>>(result);
            Assert.IsType<NotFoundResult>(notFound.Result);
        }

        [Fact]
        public async Task UpdateStatus_UpdatesAndReturnsNoContent()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateStatus_UpdatesAndReturnsNoContent));
            var status = new StoreStatus { IsOpen = false };
            context.StoreStatus.Add(status);
            context.SaveChanges();

            var controller = new StoreStatusController(context);

            var updateDto = new StoreStatusDto
            {
                Id = status.Id,
                IsOpen = true
            };

            // Act
            var result = await controller.UpdateStatus(updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updated = context.StoreStatus.First(s => s.Id == status.Id);
            Assert.True(updated.IsOpen);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsNotFound_IfMissing()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateStatus_ReturnsNotFound_IfMissing));
            var controller = new StoreStatusController(context);

            var updateDto = new StoreStatusDto
            {
                Id = 9999,
                IsOpen = true
            };

            // Act
            var result = await controller.UpdateStatus(updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
