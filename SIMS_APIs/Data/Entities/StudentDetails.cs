namespace SIMS.Data.Entities
{
    public class StudentDetails
    {
        private Guid ID;
        private Guid AccountID;
        private Guid MajorID;
        private string AdmissionCourse;

        public StudentDetails()
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

        public Guid GetAccountID()
        {
            return this.AccountID;
        }

        public void SetAccountID(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public Guid GetMajorID()
        {
            return this.MajorID;
        }

        public void SetMajorID(Guid majorID)
        {
            this.MajorID = majorID;
        }

        public string GetAdmissionCourse()
        {
            return this.AdmissionCourse;
        }

        public void SetAdmissionCourse(string admissionCourse)
        {
            this.AdmissionCourse = admissionCourse;
        }
    }
}
