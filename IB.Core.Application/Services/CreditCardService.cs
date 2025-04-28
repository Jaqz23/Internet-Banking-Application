using AutoMapper;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Services
{
    public class CreditCardService : GenericService<SaveCreditCardViewModel, CreditCardViewModel, CreditCard>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;

        public CreditCardService(ICreditCardRepository creditCardRepository, IMapper mapper)
            : base(creditCardRepository, mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }

        public async Task<List<CreditCardViewModel>> GetByUserIdAsync(string userId)
        {
            var entities = await _creditCardRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<CreditCardViewModel>>(entities);
        }

        public async Task<bool> ExistsByCardNumber(string cardNumber)
        {
            return await _creditCardRepository.ExistsByCardNumberAsync(cardNumber);
        }

        public async Task<int?> GetCardIdByNumberAsync(string cardNumber)
        {
            var card = await _creditCardRepository.GetByCardNumberAsync(cardNumber);
            return card?.Id;
        }

        public async Task<CreditCardViewModel?> GetByIdAsync(int id)
        {
            var creditCard = await _creditCardRepository.GetByIdAsync(id);
            return _mapper.Map<CreditCardViewModel?>(creditCard);
        }
    }
}
