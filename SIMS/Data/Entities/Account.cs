namespace SIMS.Data.Entities
{
    public class Account
    {
        private Guid ID;
        private string MemberCode;
        public string Email { get; set; }
        private DateTime CreateDate;
        private DateTime UpdateDate;

        public Account()
        {
            ID = Guid.NewGuid();
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        public Guid GetID()
        {
            return this.ID;
        }

        public void SetID(Guid id)
        {
            this.ID = id;
        }

        public string GetMemberCode()
        {
            return this.MemberCode;
        }

        public void SetMemberCode(string memberCode)
        {
            this.MemberCode = memberCode;
        }

        public DateTime GetCreateDate()
        {
            return this.CreateDate;
        }

        public void SetCreateDate(DateTime createDate)
        {
            this.CreateDate = createDate;
        }

        public DateTime GetUpdateDate()
        {
            return this.UpdateDate;
        }

        public void SetUpdateDate(DateTime updateDate)
        {
            this.UpdateDate = updateDate;
        }

        public override string ToString()
        {
            return "Account{" +
                    "ID=" + ID +
                    ", MemberCode='" + MemberCode + '\'' +
                    ", Email='" + Email + '\'' +
                    ", CreateDate=" + CreateDate +
                    ", UpdateDate=" + UpdateDate +
                    '}';
        }
    }
}
