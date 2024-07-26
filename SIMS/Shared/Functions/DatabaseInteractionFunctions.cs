using System;
using System.Net.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SIMS.Shared.Models;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SIMS.Shared.Functions
{
    public class DatabaseInteractionFunctions
    {
        private readonly ILogger<DatabaseInteractionFunctions> _logger;
        private readonly HttpClient _httpClient;
        private string API_URL = "https://localhost:7006/";

        public DatabaseInteractionFunctions(ILogger<DatabaseInteractionFunctions> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<T>> LoadData<T>(string methodURL)
        {
            IEnumerable<T> data = null;

            try
            {
                _logger.LogInformation($"Loading data from {API_URL + methodURL}");
                HttpResponseMessage response = await _httpClient.GetAsync(API_URL + methodURL);
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                data = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(responseStream);
                _logger.LogInformation("Data loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading data: {ex.Message}");
            }

            return data;
        }

        public async Task<T> LoadSingleData<T>(string methodURL)
        {
            T data = default(T);

            try
            {
                _logger.LogInformation($"Loading data from {API_URL + methodURL}");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(API_URL + methodURL);
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                data = await JsonSerializer.DeserializeAsync<T>(responseStream);
                _logger.LogInformation("Data loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading data: {ex.Message}");
            }

            return data;
        }

        public async Task<bool> DeleteData(string methodURL, Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting data at {API_URL}{methodURL}/{id}");
                HttpResponseMessage response = await _httpClient.DeleteAsync($"{API_URL}{methodURL}/{id}");
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Data deleted successfully");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting data: {ex.Message}");
                return false;
            }
        }

        public async Task<LoginResponse> ValidateUserAsync(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation($"Validating user {loginRequest.Email}");
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{API_URL}api/Auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Raw response content: {content}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, options);
                    _logger.LogInformation("User validation successful");
                    return loginResponse;
                }
                else
                {
                    _logger.LogError($"Validation failed with status code {response.StatusCode}");
                    return new LoginResponse { Successful = false, Error = "Invalid email or password" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating user: {ex.Message}");
                return new LoginResponse { Successful = false, Error = ex.Message };
            }
        }

    }
}
