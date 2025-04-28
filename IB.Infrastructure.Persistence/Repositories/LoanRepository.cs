using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Domain.Entities;
using IB.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        private readonly ApplicationContext _dbContext;

        public LoanRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Loan>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Loans
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }
    }

}
