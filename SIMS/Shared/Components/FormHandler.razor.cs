using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using SIMS.Shared.Models;
using SIMS.Shared.Functions;
using SIMS.Data.Entities;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SIMS.Shared.Components
{
    public partial class FormHandler : ComponentBase
    {
        [Parameter]
        public string Mode { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string MemberCode { get; set; }

        [Parameter]
        public string Email { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Role { get; set; }

        [Parameter]
        public string photoUrl { get; set; }

        [Parameter]
        public string NotificationMessage { get; set; }

        [Parameter]
        public bool NotificationSuccess { get; set; }

        private IBrowserFile selectedFile;

        [Inject]
        private DatabaseInteractionFunctions DatabaseFunctions { get; set; }

        [Inject]
        private IWebHostEnvironment env { get; set; }

        [Inject]
        private HttpClient Http { get; set; }

        private UserInfos userInfo;

        protected override async Task OnParametersSetAsync()
        {
            if (Mode == "edit" && !string.IsNullOrEmpty(Id))
            {
                await LoadItemAsync(Id);
            }
        }

        private async Task LoadItemAsync(string id)
        {
            try
            {
                userInfo = await DatabaseFunctions.LoadSingleData<UserInfos>($"api/Admin/UserInfos/{id}");

                if (userInfo != null)
                {
                    MemberCode = userInfo.MemberCode;
                    Email = userInfo.Email;
                    Name = userInfo.Name;
                    Role = userInfo.RoleName;
                    photoUrl = userInfo.OfficialAvatar;
                }
                else
                {
                    NotificationMessage = "No data found for the given ID.";
                    NotificationSuccess = false;
                }
            }
            catch (Exception ex)
            {
                NotificationMessage = $"Error occurred: {ex.Message}";
                NotificationSuccess = false;
            }
        }

        private async Task OnPhotoUrlChanged(string newPhotoUrl)
        {
            photoUrl = newPhotoUrl;
        }

        private void OnFileSelected(IBrowserFile file)
        {
            selectedFile = file;
        }

        private async Task SaveFileAsync(IBrowserFile file)
        {
            var uploadsFolder = Path.Combine(env.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream().CopyToAsync(stream);

            photoUrl = Path.Combine("Uploads", uniqueFileName);
        }

        private async Task HandleSubmit()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Role) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(MemberCode))
            {
                NotificationMessage = "Please fill out all required fields.";
                NotificationSuccess = false;
                return;
            }

            if (selectedFile != null)
            {
                await SaveFileAsync(selectedFile);
            }
            else
            {
                photoUrl = "https://cdn-icons-png.freepik.com/256/7666/7666584.png";
            }

            var formData = new MultipartFormDataContent
            {
                { new StringContent(MemberCode), "memberCode" },
                { new StringContent(Email), "email" },
                { new StringContent(Name), "name" },
                { new StringContent(Role), "role" },
                { new StringContent(photoUrl), "imagePath" }
            };

            HttpResponseMessage result;

            try
            {
                if (Mode == "edit" && !string.IsNullOrEmpty(Id))
                {
                    var updateEndpoint = $"https://localhost:7006/api/Admin/UpdateUserInfos/{Id}";
                    result = await Http.PutAsync(updateEndpoint, formData);
                }
                else
                {
                    var addEndpoint = "https://localhost:7006/api/Admin/AddAccount";
                    result = await Http.PostAsync(addEndpoint, formData);
                }

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    NotificationMessage = "Operation successful!";
                    NotificationSuccess = true;
                }
                else if (result.IsSuccessStatusCode)
                {
                    var responseContent = await result.Content.ReadFromJsonAsync<ApiResponse>();
                    NotificationMessage = responseContent.Message;
                    NotificationSuccess = responseContent.Success;
                }
                else
                {
                    var responseContent = await result.Content.ReadAsStringAsync();
                    NotificationMessage = $"Failed to process request. Status code: {result.StatusCode}, Response: {responseContent}";
                    NotificationSuccess = false;
                }
            }
            catch (Exception ex)
            {
                NotificationMessage = $"Exception occurred: {ex.Message}";
                NotificationSuccess = false;
            }
        }

        public class ApiResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
