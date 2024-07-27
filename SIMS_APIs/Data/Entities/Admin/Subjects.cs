namespace SIMS.Data.Entities.Admin
{
    public class Subjects
    {
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Slots { get; set; }
        public decimal Fee { get; set; }
    }
}
