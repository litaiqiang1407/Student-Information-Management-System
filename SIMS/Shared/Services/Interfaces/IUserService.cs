using Microsoft.AspNetCore.Components.Forms;
namespace SIMS.Data.Entities;

public interface IUserService
{
    Task<UserInfos> GetUserInfoAsync(string id);
    Task<bool> AddUserInfoAsync(UserInfos userInfo, Stream fileStream, string fileName);
    Task<bool> UpdateUserInfoAsync(string id, UserInfos userInfo, Stream fileStream, string fileName);
}

public interface IFileService
{
    Task<string> SaveFileAsync(IBrowserFile file);
}

public interface INotificationService
{
    void Notify(string message, bool isSuccess);
}
