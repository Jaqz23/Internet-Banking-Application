using IB.Core.Application.ViewModels.SavingsAccount;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Services
{
    public interface ISavingsAccountService : IGenericService<SaveSavingsAccountViewModel, SavingsAccountViewModel, SavingsAccount>
    {
        Task<List<SavingsAccountViewModel>> GetByUserIdAsync(string userId);
        Task<bool> ExistsByAccountNumber(string accountNumber);
        Task TransferAmountToPrincipal(string userId, decimal amount);
        Task<SavingsAccountViewModel?> GetByAccountNumberAsync(string accountNumber);
        Task<int?> GetAccountIdByNumberAsync(string accountNumber);
        Task<SavingsAccountViewModel?> GetByIdAsync(int id);

    }
}
