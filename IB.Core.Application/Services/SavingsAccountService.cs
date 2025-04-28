using AutoMapper;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.SavingsAccount;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Services
{
    public class SavingsAccountService : GenericService<SaveSavingsAccountViewModel, SavingsAccountViewModel, SavingsAccount>, ISavingsAccountService
    {
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IMapper _mapper;

        public SavingsAccountService(ISavingsAccountRepository savingsAccountRepository, IMapper mapper)
            : base(savingsAccountRepository, mapper)
        {
            _savingsAccountRepository = savingsAccountRepository;
            _mapper = mapper;
        }

        public async Task<List<SavingsAccountViewModel>> GetByUserIdAsync(string userId)
        {
            var accounts = await _savingsAccountRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<SavingsAccountViewModel>>(accounts);
        }

        public async Task<SavingsAccountViewModel?> GetByIdAsync(int id)
        {
            var account = await _savingsAccountRepository.GetByIdAsync(id);
            return _mapper.Map<SavingsAccountViewModel?>(account);
        }

        public async Task<SavingsAccountViewModel?> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _savingsAccountRepository.GetByAccountNumberAsync(accountNumber);
            return account != null ? _mapper.Map<SavingsAccountViewModel>(account) : null;
        }

        public async Task<int?> GetAccountIdByNumberAsync(string accountNumber)
        {
            var account = await _savingsAccountRepository.GetByAccountNumberAsync(accountNumber);
            return account?.Id;
        }


        public async Task<bool> ExistsByAccountNumber(string accountNumber)
        {
            return await _savingsAccountRepository.ExistsByAccountNumber(accountNumber);
        }

        public async Task TransferAmountToPrincipal(string userId, decimal amount)
        {
            var primaryAccount = await _savingsAccountRepository.GetPrimaryAccountByUserIdAsync(userId);

            if (primaryAccount != null)
            {
                primaryAccount.Balance += amount;
                await _savingsAccountRepository.UpdateAsync(primaryAccount, primaryAccount.Id);
            }
        }
    }
}
