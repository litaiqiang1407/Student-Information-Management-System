namespace SIMS.Data.Entities
{
    public class Major
    {
        private Guid ID;
        private Guid DepartmentID;
        private string Name;       

        public Major()
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

        public Guid GetDepartmentID()
        {
            return this.DepartmentID;
        }

        public void SetDepartmentID(Guid departmentID)
        {
            this.DepartmentID = departmentID;
        }
    }
}
