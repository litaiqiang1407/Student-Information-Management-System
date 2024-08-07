using SIMS.Data.Entities.Enums;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace SIMS.Data.Entities
{
    public class UserInfos
    {
        public int AccountID { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PersonalAvatar { get; set; }
        public string PersonalPhone { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }
        public string PermanentAddress { get; set; }
        public string TemporaryAddress { get; set; }
        public string Role { get; set; }
        public string Major { get; set; }
        public string Department { get; set; }
        public string MemberCode { get; set; }
        public string Email {  get; set; }
        public string ImagePath { get; set; }
    }
}