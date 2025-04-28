using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Application.ViewModels.Loan;
using IB.Core.Application.ViewModels.Product;
using IB.Core.Application.ViewModels.SavingsAccount;

namespace IB.Core.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<int> GetTotalAssignedProducts(); // Cantidad total de productos asignados a clientes
        Task<List<SavingsAccountViewModel>> GetSavingsAccountsByUser(string userId);
        Task<List<CreditCardViewModel>> GetCreditCardsByUser(string userId);
        Task<List<LoanViewModel>> GetLoansByUser(string userId);
        Task<SaveProductViewModel> Add(SaveProductViewModel vm);
        Task Delete(int id);
    }
}
