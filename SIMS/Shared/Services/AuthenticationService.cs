using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SIMS.Shared.Models;


namespace SIMS.Shared.Services
{
    public class AuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(LoginRequest loginModel, string role)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HTTP context is not available.");
            }

            // Log response state
            if (context.Response.HasStarted)
            {
                throw new InvalidOperationException("The HTTP response has already started.");
            }

            var identity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Name, loginModel.Email),
        new Claim(ClaimTypes.Role, role)
    }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            try
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new InvalidOperationException("An error occurred while signing in.", ex);
            }
        }

    }

}
