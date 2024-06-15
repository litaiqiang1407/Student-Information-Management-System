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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Accounts()
        {
            ID = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Account{{ ID={ID}, MemberCode='{MemberCode}', Email='{Email}', CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt} }}";
        }
    }
}
