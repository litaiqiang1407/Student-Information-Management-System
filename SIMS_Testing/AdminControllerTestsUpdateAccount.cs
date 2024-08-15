using Microsoft.AspNetCore.Mvc;
using SIMS_APIs.Controllers;
using SIMS_APIs.Models;
using SIMS_APIs.Functions;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;

namespace SIMS_Testing
{
    public class AdminControllerTestsUpdateAccount
    {
        private readonly Mock<DatabaseInteraction> _mockDbInteraction;
        private readonly AdminController _controller;

        public AdminControllerTestsUpdateAccount()
        {
            _mockDbInteraction = new Mock<DatabaseInteraction>(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller = new AdminController(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller.GetType().GetField("_dbInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(_controller, _mockDbInteraction.Object);
        }

        [Fact]
        public async Task UpdateUserInfos_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var id = 1;
            var request = new UpdateAccountRequest
            {
                Email = "updated@example.com",
                Name = "John Doe Updated",
                Gender = "Male",
                Role = "Student",
                Major = "Computer Science",
            };

            _mockDbInteraction.Setup(db => db.UpdateUserInfosAsync(id, It.IsAny<UpdateAccountRequest>()))
                              .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUserInfos(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UpdateAccountResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Account updated successfully.", response.Message);
        }

        [Fact]
        public async Task UpdateUserInfos_ShouldReturnBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var id = -1; // Invalid ID
            var request = new UpdateAccountRequest
            {
                Email = "updated@example.com",
                Name = "John Doe Updated",
                Gender = "Male",
                Role = "Student",
                Major = "Computer Science",
            };

            // Act
            var result = await _controller.UpdateUserInfos(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<UpdateAccountResponse>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Invalid ID.", response.Message);
        }

        [Fact]
        public async Task UpdateUserInfos_ShouldReturnStatusCode500_WhenUpdateFails()
        {
            // Arrange
            var id = 1;
            var request = new UpdateAccountRequest
            {
                Email = "updated@example.com",
                Name = "John Doe Updated",
                Gender = "Male",
                Role = "Student",
                Major = "Computer Science",
            };

            _mockDbInteraction.Setup(db => db.UpdateUserInfosAsync(id, It.IsAny<UpdateAccountRequest>()))
                              .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUserInfos(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var response = Assert.IsType<UpdateAccountResponse>(statusCodeResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Failed to update the account.", response.Message);
        }

        [Fact]
        public async Task UpdateUserInfos_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var id = 1;
            var request = new UpdateAccountRequest
            {
                Email = "updated@example.com",
                Name = "John Doe Updated",
                Gender = "Male",
                Role = "Student",
                Major = "Computer Science",
            };

            _mockDbInteraction.Setup(db => db.UpdateUserInfosAsync(id, It.IsAny<UpdateAccountRequest>()))
                              .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateUserInfos(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var response = Assert.IsType<UpdateAccountResponse>(statusCodeResult.Value);
            Assert.False(response.Success);
            Assert.Equal("An error occurred while updating the account.", response.Message);
            Assert.Equal("Database error", response.Details);
        }
    }
}
