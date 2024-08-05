using Microsoft.AspNetCore.Identity;

namespace SIMS.Data.Entities
{
    public class Roles : IdentityRole<Guid>
    {
        public string Name { get; set; }
    }
}