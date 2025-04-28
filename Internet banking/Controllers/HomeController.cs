using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Internet_banking.Models;
using IB.Core.Application.Dtos.Account;
using IB.Core.Application.Enums;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using IB.Core.Application.Helpers;
using IB.Core.Application.Services;

namespace Internet_banking.Controllers 
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse _userViewModel;
        private readonly IPaymentService _paymentService;
        private readonly ITransactionService _transactionService;

        public HomeController(
            IProductService productService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IPaymentService paymentService,
            ITransactionService transactionService)
        {
            _productService = productService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
            _paymentService = paymentService;
            _transactionService = transactionService;
        }

        [Authorize]
        public IActionResult Index()
        {
            var isAdmin = _userViewModel.Roles.Contains(Roles.Admin.ToString());
            if (isAdmin)
            {
                return RedirectToAction("Admin");
            }

            return RedirectToAction("Client");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var users = await _userService.GetAllAsync();

            var dashboardVm = new AdminDashboardViewModel()
            {
                TotalProductsCreated = await _productService.GetTotalAssignedProducts(),
                TotalActiveUsers = users.Count(u => u.IsActive),
                TotalInactiveUsers = users.Count(u => !u.IsActive),
                TotalPaymentsMade = await _paymentService.GetPaymentsMadeAllTheTime(),
                PaymentsMadeLast24Hours = await _paymentService.GetPaymentsMadeLast24Hours(),
                TotalTransactionsMade = await _transactionService.GetTransactionsMadeAllTheTime(),
                TransactionsMadeLast24Hours = await _transactionService.GetTransactionsMadeLast24Hours()
            };

            return View("Admin", dashboardVm);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Client()
        {
            var savingsAccounts = await _productService.GetSavingsAccountsByUser(_userViewModel.Id);
            var creditCards = await _productService.GetCreditCardsByUser(_userViewModel.Id);
            var loans = await _productService.GetLoansByUser(_userViewModel.Id);

            var allProducts = savingsAccounts.Cast<object>()
                .Concat(creditCards)
                .Concat(loans)
                .ToList();

            return View("Client", allProducts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
