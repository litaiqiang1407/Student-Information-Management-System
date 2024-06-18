using System.Text.Json;

namespace SIMS.Shared.Functions
{
    public class DatabaseInteractionFunctions
    {
        private readonly ILogger<DatabaseInteractionFunctions> _logger;
        private string API_URL = "https://localhost:7006/";

        public DatabaseInteractionFunctions(ILogger<DatabaseInteractionFunctions> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<T>> LoadData<T>(string methodURL)
        {
            IEnumerable<T> data = null;

            try
            {
                _logger.LogInformation($"Loading data from {API_URL + methodURL}");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(API_URL + methodURL);
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

        public async Task<bool> DeleteData(string methodURL, Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting data at {API_URL}{methodURL}/{id}");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.DeleteAsync($"{API_URL}{methodURL}/{id}");
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
    }
}
