using Moq;
using Microsoft.AspNetCore.Mvc;
using SIMS_APIs.Controllers;
using SIMS_APIs.Models;
using SIMS_APIs.Functions;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace SIMS_Testing
{
    public class AdminControllerTestsAddAccount
    {
        private readonly Mock<DatabaseInteraction> _mockDbInteraction;
        private readonly AdminController _controller;

        public AdminControllerTestsAddAccount()
        {
            _mockDbInteraction = new Mock<DatabaseInteraction>(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller = new AdminController(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller.GetType().GetField("_dbInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(_controller, _mockDbInteraction.Object);
        }
        [Fact]
        public async Task AddAccount_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new AddAccountRequest
            {
                MemberCode = "M001",
                Email = "test@example.com",
                Name = "John Doe",
                Gender = "Male",
                Role = "Student",
                Major = "Computer Science",
            };

            _mockDbInteraction.Setup(db => db.AddAccountWithTransaction(It.IsAny<AddAccountRequest>()))
                              .ReturnsAsync(new AddAccountResponse { Success = true, Message = "Account added successfully" });

            // Act
            var result = await _controller.AddAccount(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AddAccountResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Account added successfully", response.Message);
        }

        [Fact]
        public async Task AddAccount_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required");

            var request = new AddAccountRequest
            {
                MemberCode = "M001",
                Name = "John Doe",
                Gender = "Male",
                Role = "Student",
            };

            // Act
            var result = await _controller.AddAccount(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<AddAccountResponse>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Invalid input data.", response.Message);
        }

        [Fact]
        public async Task AddAccount_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new AddAccountRequest
            {
                MemberCode = "M001",
                Email = "test@example.com",
                Name = "John Doe",
                Gender = "Male",
                Role = "Student",
            };

            _mockDbInteraction.Setup(db => db.AddAccountWithTransaction(It.IsAny<AddAccountRequest>()))
                              .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.AddAccount(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseData = Assert.IsType<AddAccountResponse>(objectResult.Value);
            Assert.False(responseData.Success);
            Assert.Equal("An error occurred while adding the account.", responseData.Message);
        }
    }
}
