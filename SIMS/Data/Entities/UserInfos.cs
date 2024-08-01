using SIMS.Data.Entities.Enums;

namespace SIMS.Data.Entities
{
    public class UserInfos
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public string? Name { get; set; } // Nullable property
        public Gender? Gender { get; set; } // Nullable property
        public DateTime DateOfBirth { get; set; } // Nullable property
        public string? PersonalAvatar { get; set; } // Nullable property
        public string? OfficialAvatar { get; set; } // Nullable property
        public string? PersonalPhone { get; set; } // Nullable property
        public string? ContactPhone1 { get; set; } // Nullable property
        public string? ContactPhone2 { get; set; } // Nullable property
        public string? PermanentAddress { get; set; } // Nullable property
        public string? TemporaryAddress { get; set; } // Nullable property
        public string? RoleName { get; set; } // Nullable property
        public string? MajorName { get; set; } // Nullable property
        public string? DepartmentName { get; set; } // Nullable property
        public string? MemberCode { get; set; } // Nullable property
        public string? Email { get; set; } // Nullable property
        public string? ImagePath { get; set; } // Nullable property
    }
}
