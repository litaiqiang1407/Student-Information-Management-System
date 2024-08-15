namespace SIMS_APIs.Models
{
    public class UpdateAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Details { get; set; } // Optional: For providing more detailed error messages if needed

        public UpdateAccountResponse()
        {
        }

        public UpdateAccountResponse(bool success, string message, string details = null)
        {
            Success = success;
            Message = message;
            Details = details;
        }
    }
}
