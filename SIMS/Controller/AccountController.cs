using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SIMS.Data;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var authenticationProperties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
        return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest();

        var claims = result.Principal.Identities
                            .FirstOrDefault().Claims.Select(claim => new
                            {
                                claim.Issuer,
                                claim.OriginalIssuer,
                                claim.Type,
                                claim.Value
                            });

        var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        if (emailClaim != null)
        {
            // Check if the user exists in the database
            var db = HttpContext.RequestServices.GetRequiredService<SIMSDbContext>();
            var user = db.Accounts.FirstOrDefault(u => u.Email == emailClaim.Value);
            if (user != null)
            {
                //// User exists
                //var claimsIdentity = new ClaimsIdentity(new[]
                //{
                //    new Claim(ClaimTypes.Name, user.Email),
                //    new Claim(ClaimTypes.Role, user.Role.Name)
                //}, CookieAuthenticationDefaults.AuthenticationScheme);

                //var authProperties = new AuthenticationProperties
                //{
                //    IsPersistent = true,
                //    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                //};

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return Redirect("/");
            }
            else
            {
                // User does not exist
                return Unauthorized();
            }
        }

        return Redirect("/");
    }
}
