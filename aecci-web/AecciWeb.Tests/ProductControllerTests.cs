using Xunit;
using AecciWeb.Controllers;
using AecciWeb.Models;
using AecciWeb.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AecciWeb.Tests
{
    public class ProductsControllerTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAll()
        {
            // Arrange
            var context = GetDbContext(nameof(GetAllProducts_ReturnsAll));
            context.Products.AddRange(
                new Product { Name = "P1", Description = "D1", Price = 1, Quantity = 1, ImageUrl = "img1" },
                new Product { Name = "P2", Description = "D2", Price = 2, Quantity = 2, ImageUrl = "img2" }
            );
            context.SaveChanges();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(okResult.Value);
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            // Arrange
            var context = GetDbContext(nameof(GetProductById_ReturnsProduct));
            var prod = new Product { Name = "P1", Description = "D1", Price = 1, Quantity = 1, ImageUrl = "img1" };
            context.Products.Add(prod);
            context.SaveChanges();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetProductById(prod.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var readDto = Assert.IsType<ProductReadDto>(okResult.Value);
            Assert.Equal(prod.Name, readDto.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_IfMissing()
        {
            // Arrange
            var context = GetDbContext(nameof(GetProductById_ReturnsNotFound_IfMissing));
            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetProductById(1234);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProduct_AddsProduct()
        {
            // Arrange
            var context = GetDbContext(nameof(CreateProduct_AddsProduct));
            var controller = new ProductsController(context);
            var dto = new ProductCreateDto
            {
                Name = "New",
                Description = "Desc",
                Price = 9.99M,
                Quantity = 7,
                ImageUrl = null // should fallback to default placeholder
            };

            // Act
            var result = await controller.CreateProduct(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var readDto = Assert.IsType<ProductReadDto>(createdResult.Value);
            Assert.Equal(dto.Name, readDto.Name);
            Assert.Equal("https://placehold.co/300x200/E0E0E0/000000?text=No+Image", readDto.ImageUrl);
            Assert.Single(context.Products);
        }

        [Fact]
        public async Task UpdateProduct_ChangesProduct()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateProduct_ChangesProduct));
            var prod = new Product { Name = "Old", Description = "OldDesc", Price = 1, Quantity = 2, ImageUrl = "img" };
            context.Products.Add(prod);
            context.SaveChanges();

            var controller = new ProductsController(context);

            var updateDto = new ProductUpdateDto
            {
                Name = "Updated",
                Description = "UpdatedDesc",
                Price = 99.99M,
                Quantity = 99,
                ImageUrl = null // should keep original
            };

            // Act
            var result = await controller.UpdateProduct(prod.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updated = context.Products.Find(prod.Id)!;
            Assert.Equal("Updated", updated.Name);
            Assert.Equal("img", updated.ImageUrl); // unchanged
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateProduct_ReturnsNotFound));
            var controller = new ProductsController(context);

            var dto = new ProductUpdateDto
            {
                Name = "Updated",
                Description = "UpdatedDesc",
                Price = 99.99M,
                Quantity = 99,
                ImageUrl = "img"
            };

            // Act
            var result = await controller.UpdateProduct(9876, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_RemovesProduct()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteProduct_RemovesProduct));
            var prod = new Product { Name = "Del", Description = "Desc", Price = 1, Quantity = 2, ImageUrl = "img" };
            context.Products.Add(prod);
            context.SaveChanges();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.DeleteProduct(prod.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Products);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_IfMissing()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteProduct_ReturnsNotFound_IfMissing));
            var controller = new ProductsController(context);

            // Act
            var result = await controller.DeleteProduct(9999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
