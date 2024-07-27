using Microsoft.AspNetCore.Identity;
using SIMS.Data.Entities.Admin;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Data.Entities
{
    public class Users : IdentityUser<Guid>
    {
        public Guid AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Accounts Account { get; set; }
    }
}
