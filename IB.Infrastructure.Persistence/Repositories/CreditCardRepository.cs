using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Domain.Entities;
using IB.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class CreditCardRepository : GenericRepository<CreditCard>, ICreditCardRepository
    {
        private readonly ApplicationContext _dbContext;

        public CreditCardRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CreditCard>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.CreditCards
                .Where(cc => cc.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCardNumberAsync(string cardNumber)
        {
            return await _dbContext.CreditCards
                .AnyAsync(cc => cc.CardNumber == cardNumber);
        }

        public async Task<CreditCard?> GetByCardNumberAsync(string cardNumber)
        {
            return await _dbContext.CreditCards
                .FirstOrDefaultAsync(cc => cc.CardNumber == cardNumber);
        }


    }

}
