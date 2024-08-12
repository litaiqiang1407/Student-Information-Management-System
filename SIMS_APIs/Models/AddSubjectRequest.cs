namespace SIMS_APIs.Models
{
    public class AddSubjectRequest
    {
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Slots { get; set; }
        public decimal Fee { get; set; }
    }
}
