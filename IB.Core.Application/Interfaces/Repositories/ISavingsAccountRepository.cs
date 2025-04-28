using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Repositories
{
    public interface ISavingsAccountRepository : IGenericRepository<SavingsAccount>
    {
        Task<List<SavingsAccount>> GetByUserIdAsync(string userId);
        Task<SavingsAccount?> GetByAccountNumberAsync(string accountNumber);
        Task<SavingsAccount> GetPrimaryAccountByUserIdAsync(string userId);
        Task<bool> ExistsByAccountNumber(string accountNumber);
       
    }
}
