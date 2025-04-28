using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Domain.Entities;
using IB.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class BeneficiaryRepository : GenericRepository<Beneficiary>, IBeneficiaryRepository
    {
        private readonly ApplicationContext _dbContext;

        public BeneficiaryRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Beneficiary?> GetByUserIdAndAccountAsync(string userId, int savingsAccountId)
        {
            return await _dbContext.Beneficiaries
                .FirstOrDefaultAsync(b => b.UserId == userId && b.SavingsAccountId == savingsAccountId);
        }

        public async Task<List<Beneficiary>> GetAllWithSavingsAccountAsync()
        {
            return await _dbContext.Beneficiaries
                                 .Include(b => b.SavingsAccount)
                                 .ToListAsync();
        }

        public async Task<List<Beneficiary>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Beneficiaries
                .Include(b => b.SavingsAccount) 
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetByIdWithAccountAsync(int beneficiaryId)
        {
            return await _dbContext.Beneficiaries
                .Include(b => b.SavingsAccount) 
                .FirstOrDefaultAsync(b => b.Id == beneficiaryId);
        }

    }

}
