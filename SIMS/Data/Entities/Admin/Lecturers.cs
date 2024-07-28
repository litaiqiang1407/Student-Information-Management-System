namespace SIMS.Data.Entities.Admin
{
    public class Lecturers
    {
        public int ID { get; set; } // Add this line
        public string MemberCode { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
