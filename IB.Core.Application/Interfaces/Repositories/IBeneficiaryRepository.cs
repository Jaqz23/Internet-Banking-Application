using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Repositories
{
    public interface IBeneficiaryRepository : IGenericRepository<Beneficiary>
    {
        Task<Beneficiary?> GetByUserIdAndAccountAsync(string userId, int savingsAccountId);
        Task<List<Beneficiary>> GetAllWithSavingsAccountAsync();
        Task<List<Beneficiary>> GetByUserIdAsync(string userId);
        Task<Beneficiary?> GetByIdWithAccountAsync(int beneficiaryId);
    }

}
