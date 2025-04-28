using AutoMapper;
using IB.Core.Application.Helpers;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Application.ViewModels.Loan;
using IB.Core.Application.ViewModels.Product;
using IB.Core.Application.ViewModels.SavingsAccount;

namespace IB.Core.Application.Services
{
    namespace IB.Core.Application.Services
    {
        public class ProductService : IProductService
        {
            private readonly ISavingsAccountRepository _savingsAccountRepository;
            private readonly ICreditCardRepository _creditCardRepository;
            private readonly ILoanRepository _loanRepository;
            private readonly IMapper _mapper;
            private readonly ISavingsAccountService _savingsAccountService;
            private readonly ICreditCardService _creditCardService;
            private readonly ILoanService _loanService;

            public ProductService(ISavingsAccountRepository savingsAccountRepository,
                                  ICreditCardRepository creditCardRepository,
                                  ILoanRepository loanRepository,
                                  IUserService userService,
                                  IMapper mapper,
                                  ISavingsAccountService savingsAccountService,
                                  ICreditCardService creditCardService,
                                  ILoanService loanService)
            {
                _savingsAccountRepository = savingsAccountRepository;
                _creditCardRepository = creditCardRepository;
                _loanRepository = loanRepository;
                _mapper = mapper;
                _savingsAccountService = savingsAccountService;
                _creditCardService = creditCardService;
                _loanService = loanService;
            }

            public async Task<int> GetTotalAssignedProducts()
            {
                var savings = await _savingsAccountRepository.GetAllAsync();
                var creditCards = await _creditCardRepository.GetAllAsync();
                var loans = await _loanRepository.GetAllAsync();

                return savings.Count + creditCards.Count + loans.Count;
            }

            public async Task<List<SavingsAccountViewModel>> GetSavingsAccountsByUser(string userId)
            {
                var accounts = await _savingsAccountRepository.GetByUserIdAsync(userId);
                return _mapper.Map<List<SavingsAccountViewModel>>(accounts);
            }

            public async Task<List<CreditCardViewModel>> GetCreditCardsByUser(string userId)
            {
                var cards = await _creditCardRepository.GetByUserIdAsync(userId);
                return _mapper.Map<List<CreditCardViewModel>>(cards);
            }

            public async Task<List<LoanViewModel>> GetLoansByUser(string userId)
            {
                var loans = await _loanRepository.GetByUserIdAsync(userId);
                return _mapper.Map<List<LoanViewModel>>(loans);
            }

            public async Task<SaveProductViewModel> Add(SaveProductViewModel vm)
            {
                string uniqueId;

                do
                {
                    uniqueId = UniqueIdGenerator.GenerateUniqueId();
                } while (await _savingsAccountService.ExistsByAccountNumber(uniqueId) ||
                         await _creditCardService.ExistsByCardNumber(uniqueId));

                if (vm.ProductType == "SavingAccount")
                {
                    var savingsAccount = new SaveSavingsAccountViewModel
                    {
                        UserId = vm.UserId,
                        AccountNumber = uniqueId,
                        Balance = vm.Balance,
                        IsPrimary = false
                    };

                    var createdAccount = await _savingsAccountService.Add(savingsAccount);
                    return _mapper.Map<SaveProductViewModel>(createdAccount);
                }
                else if (vm.ProductType == "CreditCard")
                {
                    var creditCard = new SaveCreditCardViewModel
                    {
                        UserId = vm.UserId,
                        CardNumber = uniqueId,
                        CreditLimit = vm.CreditLimit ?? 0,
                        Debt = 0
                    };

                    var createdCard = await _creditCardService.Add(creditCard);
                    return _mapper.Map<SaveProductViewModel>(createdCard);
                }
                else if (vm.ProductType == "Loan")
                {
                    var loan = new SaveLoanViewModel
                    {
                        UserId = vm.UserId,
                        Amount = vm.Amount ?? 0,
                        RemainingBalance = vm.Amount ?? 0
                    };

                    var createdLoan = await _loanService.Add(loan);
                    await _savingsAccountService.TransferAmountToPrincipal(vm.UserId, vm.Amount ?? 0);

                    return new SaveProductViewModel
                    {
                        Id = createdLoan.Id,
                        UserId = createdLoan.UserId,
                        Amount = createdLoan.Amount,
                        Debt = createdLoan.RemainingBalance,
                        ProductType = "Loan"
                    };
                }

                return null;
            }


            public async Task Delete(int id)
            {
                // Buscar si el producto es una cuenta de ahorro
                var savingsAccount = await _savingsAccountService.GetByIdSaveViewModel(id);
                if (savingsAccount != null)
                {
                    // Si es la cuenta principal, no se puede eliminar
                    if (savingsAccount.IsPrimary)
                        return;

                    // Si tiene balance, transferirlo a la cuenta principal antes de eliminar
                    if (savingsAccount.Balance > 0)
                    {
                        await _savingsAccountService.TransferAmountToPrincipal(savingsAccount.UserId, savingsAccount.Balance);
                    }

                    await _savingsAccountService.Delete(id);
                    return;
                }

                // Buscar si el producto es una tarjeta de credito
                var creditCard = await _creditCardService.GetByIdSaveViewModel(id);
                if (creditCard != null)
                {
                    // Si tiene deuda, no se puede eliminar
                    if (creditCard.Debt > 0)
                        return;

                    await _creditCardService.Delete(id);
                    return;
                }

                // Buscar si el producto es un prestamo
                var loan = await _loanService.GetByIdSaveViewModel(id);
                if (loan != null)
                {
                    // Si tiene deuda, no se puede eliminar
                    if (loan.RemainingBalance > 0)
                        return;

                    await _loanService.Delete(id);
                }
            }

        }
    }

}
