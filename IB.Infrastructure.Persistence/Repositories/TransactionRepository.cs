using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Domain.Entities;
using IB.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly ApplicationContext _dbContext;

        public TransactionRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Transaction>> GetAllByUserIdAsync(string userId)
        {
            return await _dbContext.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetTransactionsLast24Hours()
        {
            DateTime last24Hours = DateTime.UtcNow.AddHours(-24);
            return await _dbContext.Transactions
                .Where(t => t.Date >= last24Hours)
                .CountAsync();
        }

        public async Task<int> GetTotalTransactions()
        {
            return await _dbContext.Transactions.CountAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByTypeAsync(string userId, string type)
        {
            return await _dbContext.Transactions
                .Where(t => t.UserId == userId &&
                    ((type == "Express" && t.SavingsAccountId != null && t.BeneficiaryId == null) ||
                    (type == "CreditCard" && t.CreditCardId != null) ||
                    (type == "Loan" && t.LoanId != null) ||
                    (type == "Beneficiary" && t.BeneficiaryId != null)))
                .ToListAsync();
        }

        public async Task<int> GetTotalPayments()
        {
            return await _dbContext.Transactions
                .Where(t => t.CreditCardId != null || t.LoanId != null || t.BeneficiaryId != null)
                .CountAsync();
        }

        public async Task<int> GetPaymentsLast24Hours()
        {
            DateTime last24Hours = DateTime.UtcNow.AddHours(-24);
            return await _dbContext.Transactions
                .Where(t => (t.CreditCardId != null || t.LoanId != null || t.BeneficiaryId != null) && t.Date >= last24Hours)
                .CountAsync();
        }

    }

}
