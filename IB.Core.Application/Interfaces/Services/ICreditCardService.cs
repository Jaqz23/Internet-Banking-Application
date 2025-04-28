using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Services
{
    public interface ICreditCardService : IGenericService<SaveCreditCardViewModel, CreditCardViewModel, CreditCard>
    {
        Task<List<CreditCardViewModel>> GetByUserIdAsync(string userId);
        Task<bool> ExistsByCardNumber(string cardNumber);
        Task<int?> GetCardIdByNumberAsync(string cardNumber);
        Task<CreditCardViewModel?> GetByIdAsync(int id);

    }
}
