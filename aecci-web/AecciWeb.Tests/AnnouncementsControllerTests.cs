using Xunit;
using AecciWeb.Controllers;
using AecciWeb.Models;
using AecciWeb.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AecciWeb.Tests
{
    public class AnnouncementsControllerTests
    {
        
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAllAnnouncements_ReturnsAll()
        {
            // Arrange
            var context = GetDbContext(nameof(GetAllAnnouncements_ReturnsAll));
            context.Announcements.Add(new Announcement
            {
                Title = "Test 1",
                Body = "Body 1",
                ImageUrl = "img1.png",
                IsUp = true,
                PublishedDate = System.DateTime.UtcNow
            });
            context.Announcements.Add(new Announcement
            {
                Title = "Test 2",
                Body = "Body 2",
                ImageUrl = "img2.png",
                IsUp = false,
                PublishedDate = System.DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.GetAllAnnouncements();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var announcements = Assert.IsAssignableFrom<IEnumerable<AnnouncementReadDto>>(okResult.Value);
            Assert.Equal(2, announcements.Count());
        }

        [Fact]
        public async Task GetActiveAnnouncements_ReturnsOnlyActive()
        {
            // Arrange
            var context = GetDbContext(nameof(GetActiveAnnouncements_ReturnsOnlyActive));
            context.Announcements.Add(new Announcement
            {
                Title = "Active",
                Body = "Active Body",
                ImageUrl = "active.png",
                IsUp = true,
                PublishedDate = System.DateTime.UtcNow
            });
            context.Announcements.Add(new Announcement
            {
                Title = "Inactive",
                Body = "Inactive Body",
                ImageUrl = "inactive.png",
                IsUp = false,
                PublishedDate = System.DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.GetActiveAnnouncements();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var announcements = Assert.IsAssignableFrom<IEnumerable<AnnouncementReadDto>>(okResult.Value);
            Assert.Single(announcements);
            Assert.Equal("Active", announcements.First().Title);
        }

        [Fact]
        public async Task GetAnnouncementById_ReturnsOk_WhenAnnouncementExists()
        {
            // Arrange
            var context = GetDbContext(nameof(GetAnnouncementById_ReturnsOk_WhenAnnouncementExists));
            var announcement = new Announcement { Id = 1, Title = "Test Title", Body = "Test Body", IsUp = true, PublishedDate = DateTime.UtcNow };
            context.Announcements.Add(announcement);
            await context.SaveChangesAsync();
            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.GetAnnouncementById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<AnnouncementReadDto>(okResult.Value);

            Assert.Equal(1, returnedDto.Id);
            Assert.Equal("Test Title", returnedDto.Title);
        }

        [Fact]
        public async Task GetAnnouncementById_ReturnsNotFound_WhenAnnouncementDoesNotExist()
        {
            // Arrange
            var context = GetDbContext(nameof(GetAnnouncementById_ReturnsNotFound_WhenAnnouncementDoesNotExist));
            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.GetAnnouncementById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateAnnouncement_AddsAnnouncement()
        {
            // Arrange
            var context = GetDbContext(nameof(CreateAnnouncement_AddsAnnouncement));
            var controller = new AnnouncementsController(context);

            var newDto = new AnnouncementCreateDto
            {
                Title = "Nuevo",
                Body = "Cuerpo",
                ImageUrl = "img.png"
            };

            // Act
            var result = await controller.CreateAnnouncement(newDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var readDto = Assert.IsAssignableFrom<AnnouncementReadDto>(createdResult.Value);
            Assert.Equal("Nuevo", readDto.Title);

            // Confirm it’s actually in the database
            Assert.Equal(1, await context.Announcements.CountAsync()); 
        }

        [Fact]
        public async Task CreateAnnouncement_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var context = GetDbContext(nameof(CreateAnnouncement_ReturnsBadRequest_WhenModelIsInvalid));
            var controller = new AnnouncementsController(context);
            var createDto = new AnnouncementCreateDto { Title = "", Body = "New Body" }; 

            
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.CreateAnnouncement(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }



        [Fact]
        public async Task UpdateAnnouncement_ReturnsNoContent_WhenAnnouncementExistsAndModelIsValid()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateAnnouncement_ReturnsNoContent_WhenAnnouncementExistsAndModelIsValid));
            var originalAnnouncement = new Announcement { Id = 1, Title = "Original", Body = "Original Body", ImageUrl = "original.png", IsUp = true, PublishedDate = DateTime.UtcNow };
            context.Announcements.Add(originalAnnouncement);
            await context.SaveChangesAsync(); 

            var controller = new AnnouncementsController(context);
            var updateDto = new AnnouncementUpdateDto
            {
                Title = "Updated Title",
                Body = "Updated Body",
                ImageUrl = "updated.png",
                IsUp = false 
            };

            // Act
            var result = await controller.UpdateAnnouncement(originalAnnouncement.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result); 

            
            var updatedAnnouncement = await context.Announcements.FindAsync(originalAnnouncement.Id);
            Assert.NotNull(updatedAnnouncement);
            Assert.Equal("Updated Title", updatedAnnouncement.Title);
            Assert.Equal("Updated Body", updatedAnnouncement.Body);
            Assert.Equal("updated.png", updatedAnnouncement.ImageUrl);
            Assert.False(updatedAnnouncement.IsUp); 
        }

        [Fact]
        public async Task UpdateAnnouncement_ReturnsNotFound_WhenAnnouncementDoesNotExist()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateAnnouncement_ReturnsNotFound_WhenAnnouncementDoesNotExist)); 
            var controller = new AnnouncementsController(context);
            var updateDto = new AnnouncementUpdateDto
            {
                Title = "Non Existent",
                Body = "Body",
                ImageUrl = "img.png",
                IsUp = true
            };

            // Act
            var result = await controller.UpdateAnnouncement(999, updateDto); 

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAnnouncement_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateAnnouncement_ReturnsBadRequest_WhenModelIsInvalid));
            var originalAnnouncement = new Announcement { Id = 1, Title = "Original", Body = "Original Body", ImageUrl = "original.png", IsUp = true, PublishedDate = DateTime.UtcNow };
            context.Announcements.Add(originalAnnouncement);
            await context.SaveChangesAsync();

            var controller = new AnnouncementsController(context);
            var updateDto = new AnnouncementUpdateDto
            {
                Title = "", 
                Body = "Updated Body",
                ImageUrl = "updated.png",
                IsUp = true
            };

            
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.UpdateAnnouncement(originalAnnouncement.Id, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result); 

            
            var unchangedAnnouncement = await context.Announcements.AsNoTracking().FirstOrDefaultAsync(a => a.Id == originalAnnouncement.Id);
            Assert.Equal("Original", unchangedAnnouncement.Title); 
        }



        [Fact]
        public async Task DeleteAnnouncement_RemovesAnnouncement()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteAnnouncement_RemovesAnnouncement));
            var announcement = new Announcement
            {
                Title = "ToDelete",
                Body = "Body",
                ImageUrl = "img.png",
                IsUp = true,
                PublishedDate = System.DateTime.Now
            };
            context.Announcements.Add(announcement);
            await context.SaveChangesAsync();

            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.DeleteAnnouncement(announcement.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(await context.Announcements.ToListAsync()); 
        }

        [Fact]
        public async Task DeleteAnnouncement_ReturnsNotFound_WhenAnnouncementDoesNotExist()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteAnnouncement_ReturnsNotFound_WhenAnnouncementDoesNotExist)); 
            var controller = new AnnouncementsController(context);

            // Act
            var result = await controller.DeleteAnnouncement(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}