using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Repositories
{
    public interface ICreditCardRepository : IGenericRepository<CreditCard>
    {
        Task<List<CreditCard>> GetByUserIdAsync(string userId);
        Task<bool> ExistsByCardNumberAsync(string cardNumber);
        Task<CreditCard?> GetByCardNumberAsync(string cardNumber);
    }

}
