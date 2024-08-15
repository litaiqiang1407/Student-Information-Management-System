using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using SIMS.Data.Entities.Enums;
using SIMS.Data.Entities;
using SIMS_APIs.Models;
using SIMS_Data = SIMS.Data.Entities;
using SIMS_API = SIMS_APIs;


namespace SIMS_Testing
{
    public class AdminControllerTestsProfile
    {
        private readonly Mock<SIMS_APIs.Functions.DatabaseInteraction> _mockDbInteraction;
        private readonly SIMS_APIs.Controllers.AdminController _controller;

        public AdminControllerTestsProfile()
        {
            _mockDbInteraction = new Mock<SIMS_APIs.Functions.DatabaseInteraction>(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller = new SIMS_APIs.Controllers.AdminController(Mock.Of<IConfiguration>(), Mock.Of<IWebHostEnvironment>());
            _controller.GetType().GetField("_dbInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(_controller, _mockDbInteraction.Object);
        }

        [Fact]
        public async Task GetUserInfoById_ShouldReturnOk_WhenDataIsFound()
        {
            // Arrange
            int testId = 1;
            var dataTable = CreateUserDataTable();

            var row = dataTable.NewRow();
            PopulateDataRow(row);
            dataTable.Rows.Add(row);

            _mockDbInteraction.Setup(db => db.GetData(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                              .ReturnsAsync(dataTable);

            // Act
            var result = await _controller.GetUserInfoById(testId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userInfos = Assert.IsType<UserInfos>(okResult.Value); // Đảm bảo UserInfos là đúng loại
            Assert.Equal("John Doe", userInfos.Name);
            Assert.Equal(Gender.Male, userInfos.Gender);  // Đảm bảo sử dụng enum đúng cách
            Assert.Equal(new DateTime(2000, 1, 1), userInfos.DateOfBirth);
            Assert.Equal("avatar.jpg", userInfos.PersonalAvatar);
            Assert.Equal("official_avatar.jpg", userInfos.ImagePath); // Sửa tên thuộc tính nếu cần
            Assert.Equal("1234567890", userInfos.PersonalPhone);
            Assert.Equal("0987654321", userInfos.ContactPhone1);
            Assert.Equal("1122334455", userInfos.ContactPhone2);
            Assert.Equal("123 Main St", userInfos.PermanentAddress);
            Assert.Equal("456 Side St", userInfos.TemporaryAddress);
            Assert.Equal("john.doe@example.com", userInfos.Email);
            Assert.Equal("Student", userInfos.Role);
            Assert.Equal("Computer Science", userInfos.Major);
            Assert.Equal("Engineering", userInfos.Department);
            Assert.Equal("M001", userInfos.MemberCode);
        }

        [Fact]
        public async Task GetUserInfoById_ShouldReturnNotFound_WhenDataIsNotFound()
        {
            // Arrange
            int testId = 1;
            var dataTable = CreateUserDataTable(); // DataTable with no rows

            _mockDbInteraction.Setup(db => db.GetData(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                              .ReturnsAsync(dataTable);

            // Act
            var result = await _controller.GetUserInfoById(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUserInfoById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int testId = 1;
            _mockDbInteraction.Setup(db => db.GetData(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                              .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetUserInfoById(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            // Assuming you have an error response type
            var responseData = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal("An error occurred while retrieving user information.", responseData.Message);
        }

        private DataTable CreateUserDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("AccountID", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("Gender", typeof(string));
            dataTable.Columns.Add("DateOfBirth", typeof(DateTime));
            dataTable.Columns.Add("PersonalAvatar", typeof(string));
            dataTable.Columns.Add("OfficialAvatar", typeof(string));
            dataTable.Columns.Add("PersonalPhone", typeof(string));
            dataTable.Columns.Add("ContactPhone1", typeof(string));
            dataTable.Columns.Add("ContactPhone2", typeof(string));
            dataTable.Columns.Add("PermanentAddress", typeof(string));
            dataTable.Columns.Add("TemporaryAddress", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("RoleName", typeof(string));
            dataTable.Columns.Add("MajorName", typeof(string));
            dataTable.Columns.Add("DepartmentName", typeof(string));
            dataTable.Columns.Add("MemberCode", typeof(string));
            return dataTable;
        }

        private void PopulateDataRow(DataRow row)
        {
            row["AccountID"] = 1;
            row["UserName"] = "John Doe";
            row["Gender"] = "Male";
            row["DateOfBirth"] = new DateTime(2000, 1, 1);
            row["PersonalAvatar"] = "avatar.jpg";
            row["OfficialAvatar"] = "official_avatar.jpg";
            row["PersonalPhone"] = "1234567890";
            row["ContactPhone1"] = "0987654321";
            row["ContactPhone2"] = "1122334455";
            row["PermanentAddress"] = "123 Main St";
            row["TemporaryAddress"] = "456 Side St";
            row["Email"] = "john.doe@example.com";
            row["RoleName"] = "Student";
            row["MajorName"] = "Computer Science";
            row["DepartmentName"] = "Engineering";
            row["MemberCode"] = "M001";
        }
    }
}
