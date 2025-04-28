using IB.Core.Application.ViewModels.Loan;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Services
{
    public interface ILoanService : IGenericService<SaveLoanViewModel, LoanViewModel, Loan>
    {
        Task<List<LoanViewModel>> GetByUserIdAsync(string userId);
        Task<LoanViewModel?> GetByIdAsync(int id);
    }
}
