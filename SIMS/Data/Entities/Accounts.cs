using System;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Data.Entities
{
    public class Accounts
    {
        [Key]
        public Guid ID { get; set; }

        public string MemberCode { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public Accounts()
        {
            ID = Guid.NewGuid();
            //CreatedAt = DateTime.Now;
            //UpdatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Account{{ ID={ID}, MemberCode='{MemberCode}', Email='{Email}', CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt} }}";
        }
    }
}
