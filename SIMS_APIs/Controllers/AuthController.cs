using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using SIMS_APIs.Functions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SIMS.Data.Entities;
using SIMS_APIs.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseInteraction _databaseInteraction;
        private readonly IConfiguration _configuration;

        public AuthController(DatabaseInteraction databaseInteraction, IConfiguration configuration)
        {
            _databaseInteraction = databaseInteraction;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine("Login request received");
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { Successful = false, Error = "Invalid data" });
            }

            bool isValidUser = await _databaseInteraction.ValidateUserAsync(request.Email, request.Password);
            if (!isValidUser)
            {
                return Unauthorized(new LoginResponse { Successful = false, Error = "Invalid email or password" });
            }

            var userInfo = await _databaseInteraction.GetUserInfoAsync(request.Email);
            if (userInfo == null)
            {
                return NotFound(new LoginResponse { Successful = false, Error = "User information not found" });
            }

            Console.WriteLine(userInfo.Name);


            return Ok(new LoginResponse { Successful = true, UserInfo = userInfo });
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok(new { Message = "Logout successful" });
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, new { Error = "An error occurred during logout", Details = ex.Message });
            }
        }
    }
}