namespace SIMS_APIs.Models
{
    public class UpdateCourseRequest
    {
        public string Subject { get; set; }
        public string Department { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public string? Class { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
