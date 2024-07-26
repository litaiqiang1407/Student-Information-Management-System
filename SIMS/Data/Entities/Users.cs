using Microsoft.AspNetCore.Identity;
using SIMS.Data.Entities.Admin;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Data.Entities
{
    public class Users : IdentityUser<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
