using SIMS.Data.Entities.Enums;

namespace SIMS.Data.Entities
{
    public class Enrollment
    {
        private Guid ID;
        private Guid AccountID;
        private Guid CourseID;
        private Grade Grade;
        private string Status;

        public Enrollment()
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

        public Guid GetCourseID()
        {
            return this.CourseID;
        }

        public void SetCourseID(Guid courseID)
        {
            this.CourseID = courseID;
        }

        public Grade GetGrade()
        {
            return this.Grade;
        }

        public void SetGrade(Grade grade)
        {
            this.Grade = grade;
        }

        public string GetStatus()
        {
            return this.Status;
        }

        public void SetStatus(string status)
        {
            this.Status = status;
        }
    }
}
