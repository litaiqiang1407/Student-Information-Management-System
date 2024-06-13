namespace SIMS.Data.Entities
{
    public class Subject
    {
        private Guid ID;
        private string SubjectCode;
        private string Name;
        private string Credits;
        private string Slots;
        private string Fee;

        public Subject()
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

        public string GetSubjectCode()
        {
            return this.SubjectCode;
        }

        public void SetSubjectCode(string subjectCode)
        {
            this.SubjectCode = subjectCode;
        }

        public string GetName()
        {
            return this.Name;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public string GetCredits()
        {
            return this.Credits;
        }

        public void SetCredits(string credits)
        {
            this.Credits = credits;
        }

        public string GetSlots()
        {
            return this.Slots;
        }

        public void SetSlots(string slots)
        {
            this.Slots = slots;
        }

        public string GetFee()
        {
            return this.Fee;
        }

        public void SetFee(string fee)
        {
            this.Fee = fee;
        }
    }
}
