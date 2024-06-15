namespace SIMS.Data.Entities
{
    public class Departments
    {
        private Guid ID;
        private string Name;     

        public Departments()
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
    }
}
