using IB.Core.Application.ViewModels.Beneficiary;
using IB.Core.Domain.Entities;


namespace IB.Core.Application.Interfaces.Services
{
    public interface IBeneficiaryService : IGenericService<SaveBeneficiaryViewModel, BeneficiaryViewModel, Beneficiary>
    {
        Task<List<BeneficiaryViewModel>> GetAllBeneficiaryViewModel();
        Task<bool> ExistsByAccountNumber(string accountNumber, string userId);
        Task<int?> GetBeneficiaryIdByAccountNumber(string accountNumber, string userId);
        Task<BeneficiaryViewModel?> GetByIdAsync(int id);
        Task<List<BeneficiaryViewModel>> GetByUserIdAsync(string userId);
    }
}
