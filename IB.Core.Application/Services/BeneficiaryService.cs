using AutoMapper;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.Beneficiary;
using IB.Core.Domain.Entities;


namespace IB.Core.Application.Services
{
    public class BeneficiaryService : GenericService<SaveBeneficiaryViewModel, BeneficiaryViewModel, Beneficiary>, IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public BeneficiaryService(
            IBeneficiaryRepository beneficiaryRepository,
            ISavingsAccountRepository savingsAccountRepository,
            IUserService userService,
            IMapper mapper)
            : base(beneficiaryRepository, mapper)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _savingsAccountRepository = savingsAccountRepository;
            _userService = userService;
            _mapper = mapper;
        }


        public async Task<List<BeneficiaryViewModel>> GetAllBeneficiaryViewModel()
        {
            var beneficiaries = await _beneficiaryRepository.GetAllWithSavingsAccountAsync();
            return _mapper.Map<List<BeneficiaryViewModel>>(beneficiaries);
        }

        public async Task<BeneficiaryViewModel?> GetByIdAsync(int id)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdWithAccountAsync(id);
            return _mapper.Map<BeneficiaryViewModel>(beneficiary);
        }

        public async Task<List<BeneficiaryViewModel>> GetByUserIdAsync(string userId)
        {
            var beneficiaries = await _beneficiaryRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<BeneficiaryViewModel>>(beneficiaries);
        }

        public async Task<bool> ExistsByAccountNumber(string accountNumber, string userId)
        {
            var savingsAccount = await _savingsAccountRepository.GetByAccountNumberAsync(accountNumber);
            if (savingsAccount == null) return false;

            var beneficiary = await _beneficiaryRepository.GetByUserIdAndAccountAsync(userId, savingsAccount.Id);
            return beneficiary != null;
        }

        public async Task<int?> GetBeneficiaryIdByAccountNumber(string accountNumber, string userId)
        {
            var savingsAccount = await _savingsAccountRepository.GetByAccountNumberAsync(accountNumber);
            if (savingsAccount == null) return null;

            var beneficiary = await _beneficiaryRepository.GetByUserIdAndAccountAsync(userId, savingsAccount.Id);
            return beneficiary?.Id;
        }


        public override async Task<SaveBeneficiaryViewModel> Add(SaveBeneficiaryViewModel vm)
        {
            // Verificar si el numero de cuenta ingresado existe
            var savingsAccount = await _savingsAccountRepository.GetByAccountNumberAsync(vm.AccountNumberBeneficiary);
            if (savingsAccount == null)
            {
                vm.HasError = true;
                vm.Error = "El número de cuenta ingresado no existe.";
                return vm;
            }

            // Validar que el usuario no se agregue a si mismo como beneficiario
            if (savingsAccount.UserId == vm.UserOwnerId)
            {
                vm.HasError = true;
                vm.Error = "No puedes agregarte a ti mismo como beneficiario.";
                return vm;
            }

            // Verificar si el beneficiario ya esta agregado
            var existingBeneficiary = await _beneficiaryRepository.GetByUserIdAndAccountAsync(vm.UserOwnerId!, savingsAccount.Id);
            if (existingBeneficiary != null)
            {
                vm.HasError = true;
                vm.Error = "Este beneficiario ya está agregado.";
                return vm;
            }

            // Obtener el propietario de la cuenta
            var user = await _userService.GetByIdAsync(savingsAccount.UserId);
            if (user == null)
            {
                vm.HasError = true;
                vm.Error = "El propietario de la cuenta no fue encontrado.";
                return vm;
            }

            try
            {
                var beneficiary = new Beneficiary
                {
                    UserId = vm.UserOwnerId!,
                    SavingsAccountId = savingsAccount.Id,
                    BeneficiaryName = $"{user.FirstName} {user.LastName}"
                };

                await _beneficiaryRepository.AddAsync(beneficiary);
                return _mapper.Map<SaveBeneficiaryViewModel>(beneficiary);
            }
            catch (Exception ex)
            {
                vm.HasError = true;
                vm.Error = "Ocurrió un error al agregar el beneficiario. Inténtalo nuevamente.";
                return vm;
            }
        }

    }

}
