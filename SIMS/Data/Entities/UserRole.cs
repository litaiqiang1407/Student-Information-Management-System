namespace SIMS.Data.Entities
{
    public class UserRole
    {
        private Guid ID;
        private Guid AccountID;
        private Guid RoleID;

        public UserRole()
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

        public Guid GetRoleID()
        {
            return this.RoleID;
        }

        public void SetRoleID(Guid roleID)
        {
            this.RoleID = roleID;
        }
    }
}
