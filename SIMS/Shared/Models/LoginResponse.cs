﻿using SIMS.Data.Entities;

namespace SIMS.Shared.Models
{
    public class LoginResponse
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public UserInfos UserInfo { get; set; }

    }
}
