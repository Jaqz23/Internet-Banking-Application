using IB.Core.Application.Dtos.Account;
using IB.Core.Application.Enums;
using IB.Core.Application.Helpers;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.SavingsAccount;
using IB.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Internet_banking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISavingsAccountService _savingsAccountService;

        public UserController(IUserService userService, ISavingsAccountService savingsAccountService)
        {
            _userService = userService;
            _savingsAccountService = savingsAccountService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View("SaveUser", new SaveUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("SaveUser", vm);
            }

            var origin = Request.Headers["origin"];
            RegisterResponse response = await _userService.RegisterAsync(vm, origin);
            if (response.HasError)
            {
                vm.HasError = response.HasError;
                vm.Error = response.Error;
                return View("SaveUser", vm);
            }

            if (vm.UserType == Roles.Client && !string.IsNullOrEmpty(response.Id))
            {
                string uniqueAccountNumber;
                do
                {
                    uniqueAccountNumber = UniqueIdGenerator.GenerateUniqueId();
                }
                while (await _savingsAccountService.ExistsByAccountNumber(uniqueAccountNumber));

                SaveSavingsAccountViewModel savingsAccountVm = new()
                {
                    UserId = response.Id,
                    Balance = (decimal)(vm.InitialAmount ?? 0),
                    IsPrimary = true,
                    AccountNumber = uniqueAccountNumber 
                };

                await _savingsAccountService.Add(savingsAccountVm);
            }


            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            SaveUserViewModel userVm = await _userService.GetByIdAsync(id);
            return View("SaveUser", userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("SaveUser", vm);
            }

            SaveUserViewModel userVm = await _userService.GetByIdAsync(vm.Id);

            if (userVm.UserType == Roles.Client && vm.InitialAmount > 0)
            {
                await _savingsAccountService.TransferAmountToPrincipal(vm.Id, (decimal)vm.InitialAmount);
            }

            await _userService.UpdateAsync(vm, vm.Id);

            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        public async Task<IActionResult> ConfirmAction(string id)
        {
            SaveUserViewModel userVm = await _userService.GetByIdAsync(id);
            return View("ConfirmAction", userVm);
        }

        [HttpPost]
        public async Task<IActionResult> ActiveUser(SaveUserViewModel vm)
        {
            await _userService.ActivateUserAsync(vm.Id);
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        [HttpPost]
        public async Task<IActionResult> InactiveUser(SaveUserViewModel vm)
        {
            await _userService.InactivateUserAsync(vm.Id);
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }
    }
}
