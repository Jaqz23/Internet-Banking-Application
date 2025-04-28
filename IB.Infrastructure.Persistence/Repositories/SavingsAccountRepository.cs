using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Domain.Entities;
using IB.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class SavingsAccountRepository : GenericRepository<SavingsAccount>, ISavingsAccountRepository
    {
        private readonly ApplicationContext _dbContext;

        public SavingsAccountRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SavingsAccount>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.SavingsAccounts
                .Where(sa => sa.UserId == userId)
                .ToListAsync();
        }

        public async Task<SavingsAccount?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbContext.SavingsAccounts
                .FirstOrDefaultAsync(sa => sa.AccountNumber == accountNumber);
        }

        public async Task<SavingsAccount> GetPrimaryAccountByUserIdAsync(string userId)
        {
            return await _dbContext.SavingsAccounts
                .FirstOrDefaultAsync(sa => sa.UserId == userId && sa.IsPrimary);
        }

        public async Task<bool> ExistsByAccountNumber(string accountNumber)
        {
            return await _dbContext.SavingsAccounts.AnyAsync(sa => sa.AccountNumber == accountNumber);
        }

    }
}
