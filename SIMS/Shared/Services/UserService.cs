using Microsoft.AspNetCore.Components.Forms;
using SIMS.Data.Entities;
using System.Net.Http.Headers;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserInfos> GetUserInfoAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<UserInfos>($"api/Admin/UserInfos/{id}");
    }

    public async Task<bool> AddUserInfoAsync(UserInfos userInfo, Stream fileStream, string fileName)
    {
        var formData = CreateFormData(userInfo, fileStream, fileName);
        var response = await _httpClient.PostAsync("api/Admin/AddAccount", formData);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserInfoAsync(string id, UserInfos userInfo, Stream fileStream, string fileName)
    {
        var formData = CreateFormData(userInfo, fileStream, fileName);
        var response = await _httpClient.PutAsync($"api/Admin/UpdateUserInfos/{id}", formData);
        return response.IsSuccessStatusCode;
    }

    private MultipartFormDataContent CreateFormData(UserInfos userInfo, Stream fileStream, string fileName)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(userInfo.MemberCode), "MemberCode");
        formData.Add(new StringContent(userInfo.Email), "Email");
        formData.Add(new StringContent(userInfo.Name), "Name");
        formData.Add(new StringContent(userInfo.RoleName), "Role");

        if (fileStream != null)
        {
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Set correct content type
            formData.Add(fileContent, "ImagePath", fileName);
        }
        else
        {
            formData.Add(new StringContent("https://cdn-icons-png.freepik.com/256/7666/7666584.png"), "ImagePath");
        }

        return formData;
    }
}

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveFileAsync(IBrowserFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.Name}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream().CopyToAsync(stream);

        return Path.Combine("Uploads", uniqueFileName);
    }
}

public class NotificationService : INotificationService
{
    public void Notify(string message, bool isSuccess)
    {
        // Implementation for displaying notifications
        Console.WriteLine($"Notification: {message}, Success: {isSuccess}");
    }
}
