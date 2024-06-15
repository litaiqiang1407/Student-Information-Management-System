using System.Text.Json;
using SIMS.Shared.Models;


namespace SIMS.Shared.Functions
{
    public class DatabaseInteractionFunctions
    {
        private string API_URL = "https://localhost:7006/";

        public async Task<IEnumerable<T>> LoadData<T>(string methodURL)
        {
            IEnumerable<T> data = null;

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(API_URL + methodURL);
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                data = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(responseStream);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Error loading data: {ex.Message}");
            }

            return data;
        }
    }
}
