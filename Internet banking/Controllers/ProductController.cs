using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.Services;
using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Application.ViewModels.Loan;
using IB.Core.Application.ViewModels.Product;
using IB.Core.Application.ViewModels.SavingsAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Internet_banking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public ProductController(
            ISavingsAccountService savingsAccountService,
            ICreditCardService creditCardService,
            ILoanService loanService,
            IUserService userService,
            IProductService productService)
        {
            _savingsAccountService = savingsAccountService;
            _creditCardService = creditCardService;
            _loanService = loanService;
            _userService = userService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string userId)
        {
            ViewBag.UserId = userId;

            var savingsAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            var creditCards = await _creditCardService.GetByUserIdAsync(userId);
            var loans = await _loanService.GetByUserIdAsync(userId);

            var user = await _userService.GetByIdAsync(userId);
            string userName = $"{user.FirstName} {user.LastName}"; 

            List<ProductsViewModel> products = new();

            products.AddRange(savingsAccounts.Select(sa => new ProductsViewModel
            {
                Id = sa.Id,
                AccountNumber = sa.AccountNumber,
                Balance = sa.Balance,
                ProductType = "Cuenta de Ahorro",
                IsPrimary = sa.IsPrimary,
                UserName = userName
            }));

            products.AddRange(creditCards.Select(cc => new ProductsViewModel
            {
                Id = cc.Id,
                AccountNumber = cc.CardNumber,
                Balance = cc.CreditLimit,
                Debt = cc.Debt,
                ProductType = "Tarjeta de Crédito",
                UserName = userName
            }));

            products.AddRange(loans.Select(l => new ProductsViewModel
            {
                Id = l.Id,
                AccountNumber = $"LOAN-{l.Id}",
                Balance = l.Amount,
                Debt = l.RemainingBalance,
                ProductType = "Préstamo",
                UserName = userName
            }));

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = vm.UserId;
                return View("Create", vm);
            }

            await _productService.Add(vm);
            return RedirectToAction("Index", new { userId = vm.UserId });
        }

        public IActionResult Create(string userId)
        {
            return View(new SaveProductViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string productType, string userId)
        {
            bool canDelete = productType switch
            {
                "Tarjeta de Crédito" => (await _creditCardService.GetByIdSaveViewModel(id)).Debt == 0,
                "Préstamo" => (await _loanService.GetByIdSaveViewModel(id)).RemainingBalance == 0,
                "Cuenta de Ahorro" => !(await _savingsAccountService.GetByIdSaveViewModel(id)).IsPrimary,
                _ => false
            };

            if (canDelete)
            {
                await _productService.Delete(id);
            }
            else
            {
                TempData["ErrorMessage"] = "No se puede eliminar este producto porque tiene deuda o es la cuenta principal.";
            }

            return RedirectToAction("Index", new { userId });
        }

    }
}
