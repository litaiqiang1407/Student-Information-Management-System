using SIMS.Data.Entities.Enums;

namespace SIMS.Data.Entities
{
    public class UserInfo
    {
        private Guid ID;
        private Guid AccountID;
        private string Name;
        private Gender Gender;
        private DateTime DateOfBirth;
        private string PersonalAvatar;
        private string OfficialAvatar;
        private string PersonalPhone;
        private string ContactPhone1;
        private string ContactPhone2;
        private string PermanentAddress;
        private string TemporaryAddress;

        public UserInfo()
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

        public string GetName()
        {
            return this.Name;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public Gender GetGender()
        {
            return this.Gender;
        }

        public void SetGender(Gender gender)
        {
            this.Gender = gender;
        }

        public DateTime GetDateOfBirth()
        {
            return this.DateOfBirth;
        }

        public void SetDateOfBirth(DateTime dateOfBirth)
        {
            this.DateOfBirth = dateOfBirth;
        }

        public string GetPersonalAvatar()
        {
            return this.PersonalAvatar;
        }

        public void SetPersonalAvatar(string personalAvatar)
        {
            this.PersonalAvatar = personalAvatar;
        }

        public string GetOfficialAvatar()
        {
            return this.OfficialAvatar;
        }

        public void SetOfficialAvatar(string officialAvatar)
        {
            this.OfficialAvatar = officialAvatar;
        }

        public string GetPersonalPhone()
        {
            return this.PersonalPhone;
        }

        public void SetPersonalPhone(string personalPhone)
        {
            this.PersonalPhone = personalPhone;
        }

        public string GetContactPhone1()
        {
            return this.ContactPhone1;
        }

        public void SetContactPhone1(string contactPhone1)
        {
            this.ContactPhone1 = contactPhone1;
        }

        public string GetContactPhone2()
        {
            return this.ContactPhone2;
        }

        public void SetContactPhone2(string contactPhone2)
        {
            this.ContactPhone2 = contactPhone2;
        }

        public string GetPermanentAddress()
        {
            return this.PermanentAddress;
        }

        public void SetPermanentAddress(string permanentAddress)
        {
            this.PermanentAddress = permanentAddress;
        }

        public string GetTemporaryAddress()
        {
            return this.TemporaryAddress;
        }

        public void SetTemporaryAddress(string temporaryAddress)
        {
            this.TemporaryAddress = temporaryAddress;
        }
    }
}
