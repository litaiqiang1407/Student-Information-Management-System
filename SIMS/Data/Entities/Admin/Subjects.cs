namespace SIMS.Data.Entities.Admin
{
    public class Subjects
    {
        public int? ID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Slots { get; set; }
        public decimal Fee { get; set; }
    }
}
