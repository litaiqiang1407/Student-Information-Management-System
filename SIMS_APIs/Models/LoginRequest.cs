﻿using System.ComponentModel.DataAnnotations;

namespace SIMS_APIs.Models
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
