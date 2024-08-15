using Moq;
using Microsoft.AspNetCore.Mvc;
using SIMS_APIs.Controllers;
using SIMS_APIs.Models; // Đảm bảo không gian tên này chính xác
using SIMS_APIs.Functions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient; // Sử dụng Microsoft.Data.SqlClient thay vì System.Data.SqlClient
using SIMS.Data.Entities;

namespace SIMS_Testing
{
    public class AdminControllerTestsAddRegistrationUser
    {
        private readonly Mock<DatabaseInteraction> _mockDbInteraction;
        private readonly AdminController _controller;

        public AdminControllerTestsAddRegistrationUser()
        {
            _mockDbInteraction = new Mock<DatabaseInteraction>(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller = new AdminController(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller.GetType().GetField("_dbInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(_controller, _mockDbInteraction.Object);
        }
        [Fact]
        public async Task AddRegistrationUser_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegistrationUsers
            {
                FullName = "John Doe",
                PhoneNumber = "1234567890",
                Birthdate = new DateTime(1990, 1, 1),
                Major = "Computer Science"
            };

            var expectedResponse = new JsonResult(new AddRegistrationUserResponse
            {
                Success = true,
                Message = "Data added successfully"
            });

            _mockDbInteraction.Setup(db => db.AddDataWithSQLQuery(It.IsAny<string>(), It.IsAny<Microsoft.Data.SqlClient.SqlParameter[]>()))
                              .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddRegistrationUser(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AddRegistrationUserResponse>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Data added successfully", response.Message);
        }
        [Fact]
        public async Task AddRegistrationUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FullName", "Full name is required");

            var request = new RegistrationUsers
            {
                PhoneNumber = "1234567890",
                Birthdate = new DateTime(1990, 1, 1),
                Major = "Computer Science"
            };

            // Act
            var result = await _controller.AddRegistrationUser(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<AddRegistrationUserResponse>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Invalid input data.", response.Message);
            Assert.NotNull(response.Details); // Ensure that Details property is properly handled
        }
    }
}
