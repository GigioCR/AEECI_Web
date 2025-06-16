using Xunit;
using AecciWeb.Controllers;
using AecciWeb.Models;
using AecciWeb.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;

namespace AecciWeb.Tests
{
    public class AuthControllerTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private IConfiguration GetFakeConfig()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtSettings:SecretKey", "THIS_IS_A_DEMO_SECRET_1234567890123456"}, // must be at least 16 chars
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawData);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var context = GetDbContext(nameof(Login_WithValidCredentials_ReturnsToken));
            var username = "testuser";
            var password = "Test1234";
            var passwordHash = ComputeSha256Hash(password);

            context.Users.Add(new User
            {
                Username = username,
                PasswordHash = passwordHash
            });
            context.SaveChanges();

            var config = GetFakeConfig();
            var controller = new AuthController(context, config);

            var loginDto = new LoginDto { Username = username, Password = password };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tokenResponse = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.Token));
            Assert.True(tokenResponse.Expiration > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var context = GetDbContext(nameof(Login_WithInvalidPassword_ReturnsUnauthorized));
            var username = "testuser";
            var correctPassword = "CorrectPass123";
            var passwordHash = ComputeSha256Hash(correctPassword);

            context.Users.Add(new User
            {
                Username = username,
                PasswordHash = passwordHash
            });
            context.SaveChanges();

            var config = GetFakeConfig();
            var controller = new AuthController(context, config);

            var loginDto = new LoginDto { Username = username, Password = "WrongPass" };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_WithUnknownUser_ReturnsUnauthorized()
        {
            // Arrange
            var context = GetDbContext(nameof(Login_WithUnknownUser_ReturnsUnauthorized));
            var config = GetFakeConfig();
            var controller = new AuthController(context, config);

            var loginDto = new LoginDto { Username = "doesnotexist", Password = "anything" };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}
