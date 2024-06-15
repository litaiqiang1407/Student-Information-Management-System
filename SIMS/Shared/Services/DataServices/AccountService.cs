using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Data.Entities;

namespace SIMS.Shared.Services.DataServices
{
    public class AccountService
    {
        private readonly SIMSDbContext _dbContext;

        public AccountService(SIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<List<string>> GetAccountColumnsAsync()
        {
            return new List<string> { "ID", "MemberCode", "Email", "CreatedAt", "UpdatedAt" };
        }
    }
}
