using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Repositories
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<List<Transaction>> GetAllByUserIdAsync(string userId);
        Task<int> GetTransactionsLast24Hours();
        Task<int> GetTotalTransactions();
        Task<List<Transaction>> GetTransactionsByTypeAsync(string userId, string type);

        Task<int> GetTotalPayments();
        Task<int> GetPaymentsLast24Hours();
    }
}
