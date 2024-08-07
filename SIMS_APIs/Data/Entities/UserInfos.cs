﻿using SIMS.Data.Entities.Enums;

namespace SIMS.Data.Entities
{
    public class UserInfos
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PersonalAvatar { get; set; }
        public string OfficialAvatar { get; set; }
        public string PersonalPhone { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }
        public string PermanentAddress { get; set; }
        public string TemporaryAddress { get; set; }
    }
}
