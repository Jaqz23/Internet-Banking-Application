using IB.Core.Application.Dtos.Account;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.User;
using Internet_banking.Middlewares;
using IB.Core.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Internet_banking.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            AuthenticationResponse userVm = await _accountService.AuthenticateAsync(new AuthenticationRequest
            {
                UserName = vm.UserName,
                Password = vm.Password
            });

            if (userVm != null && !userVm.HasError)
            {
                HttpContext.Session.Set("user", userVm);
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            else
            {
                vm.HasError = userVm.HasError;
                vm.Error = userVm.Error;
                return View(vm);
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await _accountService.SignOutAsync();
            HttpContext.Session.Remove("user");
            return RedirectToRoute(new { controller = "Auth", action = "Login" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
