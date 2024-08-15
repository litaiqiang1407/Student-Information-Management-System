//extern alias SIMS;
//extern alias SIMS_API;
//using Bunit;
//using Moq;
//using Xunit;
//using SIMS.SIMS.Pages.Admin.Account.AccountDetail;
//using SIMS.SIMS.Shared.Services;
//using SIMS.SIMS.Shared.Functions;
//using SIMS.SIMS.Data.Entities;
//using System;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using SIMS.SIMS.Data;
//using Microsoft.EntityFrameworkCore;

//namespace SIMS_Testing
//{
//    public class AccountDetailIntegrationTests : TestContext
//    {
//        private readonly SIMSDbContext _dbContext;

//        private readonly DatabaseInteractionFunctions _databaseFunctions;
//        private readonly HeaderTitleService _headerTitleService;


//        public AccountDetailIntegrationTests()
//        {
//            var serviceProvider = new ServiceCollection()
//                .AddDbContext<SIMSDbContext>(options =>
//                    options.UseSqlServer("Data Source=VUTRIEU\\SQLEXPRESS;Initial Catalog=SIMS;Integrated Security=True;Trust Server Certificate=True"))
//                .BuildServiceProvider();

//            _dbContext = serviceProvider.GetRequiredService<SIMSDbContext>();
//            _dbContext.Database.OpenConnection();
//            _dbContext.Database.EnsureCreated();

//            SeedDatabase();
//        }

//        private void SeedDatabase()
//        {
//            // Seed the database with test data
//            var userInfo = new UserInfos
//            {
//                ID = 1,
//                Name = "John Doe",
//                DateOfBirth = new DateTime(1990, 5, 1),
//                Gender = Gender.Male,
//                PersonalPhone = "123-456-7890",
//                PermanentAddress = "123 Main St",
//                ImagePath = "https://example.com/avatar.jpg",
//                Role = "Admin",
//                Major = "Computer Science",
//                Department = "IT",
//                MemberCode = "ABC123"
//            };

//            _dbContext.UserInfos.Add(userInfo);
//            _dbContext.SaveChanges();
//        }

//        [Fact]
//        public async Task AccountDetail_ShouldRenderWithCorrectData()
//        {
//            // Arrange
//            var component = RenderComponent<AccountDetail>(parameters => parameters
//                .Add(p => p.ID, 1)
//                .Add(p => p.DatabaseFunctions, _databaseFunctions)
//                .Add(p => p.HeaderTitleService, _headerTitleService));

//            // Act
//            await component.InvokeAsync(() => component.Render());

//            // Assert
//            // You should now be able to check the rendered output and component properties
//            Assert.Contains("John Doe", component.Markup); // example assertion
//        }
//    }
//}
