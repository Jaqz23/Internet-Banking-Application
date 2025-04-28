using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Repositories
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<List<Loan>> GetByUserIdAsync(string userId);
    }
}
