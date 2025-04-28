using AutoMapper;
using IB.Core.Application.Dtos.Account;
using IB.Core.Application.Enums;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.SavingsAccount;
using IB.Core.Application.ViewModels.User;

namespace IB.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountService _accountService;
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly IMapper _mapper;

        public UserService(IAccountService accountService, ISavingsAccountService savingsAccountService, IMapper mapper)
        {
            _accountService = accountService;
            _savingsAccountService = savingsAccountService;
            _mapper = mapper;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginViewModel vm)
        {
            AuthenticationRequest loginRequest = _mapper.Map<AuthenticationRequest>(vm);
            AuthenticationResponse userResponse = await _accountService.AuthenticateAsync(loginRequest);
            return userResponse;
        }

        public async Task SignOutAsync()
        {
            await _accountService.SignOutAsync();
        }

        public async Task<RegisterResponse> RegisterAsync(SaveUserViewModel vm, string origin)
        {
            RegisterRequest registerRequest = _mapper.Map<RegisterRequest>(vm);
            RegisterResponse response = await _accountService.RegisterUserAsync(registerRequest, origin);

            if (!response.HasError && vm.UserType == Roles.Client)
            {
                var uniqueAccountNumber = await GenerateUniqueAccountNumber();

                var savingsAccount = new SaveSavingsAccountViewModel
                {
                    UserId = response.Id,
                    AccountNumber = uniqueAccountNumber,
                    Balance = (decimal)(vm.InitialAmount ?? 0),
                    IsPrimary = true
                };
            }

            return response;
        }

        public async Task UpdateAsync(SaveUserViewModel vm, string id)
        {
            RegisterRequest registerRequest = _mapper.Map<RegisterRequest>(vm);
            await _accountService.UpdateUserAsync(registerRequest, id);
        }

        public async Task<SaveUserViewModel> GetByIdAsync(string id)
        {
            AuthenticationResponse userResponse = await _accountService.GetUserByIdAsync(id);
            return _mapper.Map<SaveUserViewModel>(userResponse);
        }

        public async Task<List<UserViewModel>> GetAllAsync()
        {
            var users = await _accountService.GetAllUsersAsync();
            var userViewModels = _mapper.Map<List<UserViewModel>>(users);

            foreach (var user in userViewModels)
            {
                user.SavingsAccounts = await _savingsAccountService.GetByUserIdAsync(user.Id);
            }

            return userViewModels;
        }

        public async Task ActivateUserAsync(string id)
        {
            await _accountService.ActivateUserAsync(id);
        }

        public async Task InactivateUserAsync(string id)
        {
            await _accountService.InactivateUserAsync(id);
        }

        private async Task<string> GenerateUniqueAccountNumber()
        {
            string accountNumber;
            do
            {
                accountNumber = new Random().Next(100000000, 999999999).ToString();
            }
            while (await _savingsAccountService.ExistsByAccountNumber(accountNumber));

            return accountNumber;
        }
    }
}
