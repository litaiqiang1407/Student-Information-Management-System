namespace SIMS.Data.Entities.Admin
{
    public class Majors
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public override string ToString()
        {
            return $"Id: {ID}, Name: {Name}, Department: {Department}";
        }
    }
}
