using SIMS.Data.Entities.Enums;
namespace SIMS.Data.Entities.Admin
{
    public class Courses
    {
        public int? ID { get; set; }
        public string? Subject { get; set; }
        public string? Department { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public Class? Class { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
