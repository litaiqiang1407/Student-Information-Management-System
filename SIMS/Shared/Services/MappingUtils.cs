using SIMS.Shared.Models; // Namespace của AddAccountRequest
using SIMS.Data.Entities; // Namespace của UserInfos

namespace SIMS.Shared.Services
{
    public static class MappingUtils
    {
        public static AddAccountReqest MapToAddAccountRequest(UserInfos userInfos)
        {
            return new AddAccountReqest
            {
                MemberCode = userInfos.MemberCode,
                Email = userInfos.Email,
                Name = userInfos.Name,
                Gender = userInfos.Gender?.ToString(), // Chuyển đổi Gender? thành string
                // Các thuộc tính không có trong UserInfos cần xử lý thêm
                Major = default, // Không có thuộc tính trong UserInfos
                CourseID = default, // Không có thuộc tính trong UserInfos
                Grade = string.Empty, // Không có thuộc tính trong UserInfos
                ImagePath = userInfos.ImagePath,
                Password = string.Empty, // Không có thuộc tính trong UserInfos
                DateOfBirth = userInfos.DateOfBirth,
                PersonalPhone = userInfos.PersonalPhone,
                ContactPhone1 = userInfos.ContactPhone1,
                ContactPhone2 = userInfos.ContactPhone2,
                PermanentAddress = userInfos.PermanentAddress,
                TemporaryAddress = userInfos.TemporaryAddress
            };
        }
    }
}
