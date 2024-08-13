using SIMS.Data.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SIMS_APIs.Models
{
    public class AddAccountRequest
    {
        [Required]
        public string MemberCode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Role { get; set; }

        public string? Major { get; set; }

        public string? ImagePath { get; set; }

        public string? Password { get; set; }

        public DateTime? DateOfBirth { get; set; } 

        public string? PersonalPhone { get; set; }

        public string? ContactPhone1 { get; set; }

        public string? ContactPhone2 { get; set; }

        public string? PermanentAddress { get; set; }

        public string? TemporaryAddress { get; set; }
    }
}
