namespace SIMS.Data.Entities
{
    public class Courses
    {
        private Guid ID;
        private string Name;
        private Guid DepartmentID;
        private Guid SemesterID;
        private Guid AccountID;
        private string ClassName;
        private DateTime StartDate;
        private DateTime EndDate;

        public Courses()
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

        public Guid GetSemesterID()
        {
            return this.SemesterID;
        }

        public void SetSemesterID(Guid semesterID)
        {
            this.SemesterID = semesterID;
        }

        public Guid GetAccountID()
        {
            return this.AccountID;
        }

        public void SetAccountID(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public string GetClassName()
        {
            return this.ClassName;
        }

        public void SetClassName(string className)
        {
            this.ClassName = className;
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
