namespace SIMS.Data.Entities
{
    public class Semesters
    {
        private Guid ID;
        private string Name;
        private DateTime StartDate;
        private DateTime EndDate;

        public Semesters()
        {
            ID = Guid.NewGuid();
        }

        public Guid GetID()
        {
            return this.ID;
        }

        public void SetID(Guid id)
        {
            this.ID = id;
        }

        public string GetName()
        {
            return this.Name;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public DateTime GetStartDate()
        {
            return this.StartDate;
        }

        public void SetStartDate(DateTime startDate)
        {
            this.StartDate = startDate;
        }

        public DateTime GetEndDate()
        {
            return this.EndDate;
        }

        public void SetEndDate(DateTime endDate)
        {
            this.EndDate = endDate;
        }
    }
}
