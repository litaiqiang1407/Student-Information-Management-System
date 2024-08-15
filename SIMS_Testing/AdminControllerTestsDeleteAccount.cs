using Microsoft.AspNetCore.Mvc;
using Moq;
using SIMS_APIs.Controllers;
using SIMS_APIs.Functions;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using SIMS_APIs.Models;
using static SIMS_APIs.Functions.DatabaseInteraction;

namespace SIMS_Testing
{
    public class AdminControllerTestsDeleteAccount
    {
        private readonly Mock<DatabaseInteraction> _mockDbInteraction;
        private readonly AdminController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        public AdminControllerTestsDeleteAccount()
        {
            // Mô phỏng IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("ConnectionStrings")["SIMSConnection"]).Returns("Data Source=VUTRIEU\\SQLEXPRESS;Initial Catalog=SIMS;Integrated Security=True;Trust Server Certificate=True");

            // Mô phỏng IWebHostEnvironment
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            // Khởi tạo DatabaseInteraction với các đối tượng mô phỏng
            _mockDbInteraction = new Mock<DatabaseInteraction>(_mockConfiguration.Object, _mockWebHostEnvironment.Object);
            _controller = new AdminController(_mockConfiguration.Object, _mockWebHostEnvironment.Object);

            // Sử dụng phản chiếu để gán _dbInteraction cho controller
            _controller.GetType().GetField("_dbInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(_controller, _mockDbInteraction.Object);
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnSuccess_WhenOperationIsSuccessful()
        {
            // Arrange
            int testId = 1;
            _mockDbInteraction.Setup(db => db.GetOfficialAvatarPathByAccountId(It.IsAny<int>()))
                               .ReturnsAsync("path/to/image.jpg");
            _mockDbInteraction.Setup(db => db.DeleteImage(It.IsAny<string>()))
                               .ReturnsAsync(new OperationResult { Success = true, Message = "Image deletion successful." });
            _mockDbInteraction.Setup(db => db.DeleteAccountAndRelatedData(It.IsAny<int>()))
                               .ReturnsAsync(new OkObjectResult(new DeleteAccountResponse { Success = true, Message = "Transaction committed successfully" }));

            // Act
            var result = await _controller.DeleteAccount(testId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteAccountResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Transaction committed successfully", response.Message);
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            int testId = 1;
            var expectedErrorMessage = "An error occurred: Database error";
            _mockDbInteraction.Setup(db => db.DeleteAccountAndRelatedData(It.IsAny<int>()))
                               .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteAccount(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<DeleteAccountResponse>(objectResult.Value);
            Assert.False(response.Success);
            Assert.Equal(expectedErrorMessage, response.Message);
        }
    }
}
