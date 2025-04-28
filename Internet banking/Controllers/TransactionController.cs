using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.Services;
using IB.Core.Application.ViewModels.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Internet_banking.Controllers
{
    [Authorize(Roles = "Client")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly IUserService _userService;
       
        public TransactionController(
            ITransactionService transactionService,
            ISavingsAccountService savingsAccountService,
            ICreditCardService creditCardService,
            ILoanService loanService,
            IBeneficiaryService beneficiaryService,
            IUserService userService)
        {
            _transactionService = transactionService;
            _savingsAccountService = savingsAccountService;
            _creditCardService = creditCardService;
            _loanService = loanService;
            _beneficiaryService = beneficiaryService;
            _userService = userService;
        }

        #region Pago expreso
        public async Task<IActionResult> ExpressPayment()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            ViewBag.UserAccounts = userAccounts;
            return View(new SaveTransactionViewModel { UserId = userId });
        }


        [HttpPost]
        public async Task<IActionResult> ExpressPayment(SaveTransactionViewModel vm)
        {
            var destinationAccount = await _savingsAccountService.GetByAccountNumberAsync(vm.ToAccountNumber);
            if (destinationAccount == null)
            {
                TempData["ErrorMessage"] = "El número de cuenta ingresado no existe.";
                return RedirectToAction("ExpressPayment");
            }

            var sourceAccounts = await _savingsAccountService.GetByUserIdAsync(vm.UserId);
            var sourceAccount = sourceAccounts.FirstOrDefault(a => a.Id == vm.SavingsAccountId);

            if (sourceAccount == null || sourceAccount.Balance < vm.Amount)
            {
                TempData["ErrorMessage"] = "No tienes fondos suficientes para realizar este pago.";
                return RedirectToAction("ExpressPayment");
            }

            var recipientUser = await _userService.GetByIdAsync(destinationAccount.UserId);
            string recipientName = recipientUser != null ? $"{recipientUser.FirstName} {recipientUser.LastName}" : "Desconocido";

            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = destinationAccount.AccountNumber,
                RecipientName = recipientName,
                Amount = vm.Amount
            };

            return View("ConfirmExpressPayment", confirmationVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmExpressPayment(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el pago.";

                if (string.IsNullOrEmpty(vm.RecipientName))
                {
                    var destinationAccount = await _savingsAccountService.GetByAccountNumberAsync(vm.ToAccountNumber);
                    if (destinationAccount != null)
                    {
                        var recipientUser = await _userService.GetByIdAsync(destinationAccount.UserId);
                        vm.RecipientName = recipientUser != null ? $"{recipientUser.FirstName} {recipientUser.LastName}" : "Desconocido";
                    }
                }

                return View("ConfirmPayment", vm);
            }


            var transaction = new SaveTransactionViewModel
            {
                UserId = vm.UserId,
                Amount = vm.Amount,
                ToAccountNumber = vm.ToAccountNumber,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.FromAccountNumber)
            };

            bool success = await _transactionService.ProcessExpressPayment(transaction);
            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar el pago.";
                return RedirectToAction("ExpressPayment");
            }

            TempData["SuccessMessage"] = "Pago realizado con éxito.";
            return RedirectToAction("Client", "Home");
        }


        public IActionResult CancelPaymentExpress()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("ExpressPayment");
        }

        #endregion


        #region Pago de Tarjeta de Crédito 

        public async Task<IActionResult> CreditCardPayment()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var savingsAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            var creditCards = await _creditCardService.GetByUserIdAsync(userId);

            if (savingsAccounts == null || !savingsAccounts.Any())
            {
                TempData["ErrorMessage"] = "No tienes cuentas de ahorro disponibles.";
            }

            if (creditCards == null || !creditCards.Any())
            {
                TempData["ErrorMessage"] = "No tienes tarjetas de crédito disponibles.";
            }

            ViewBag.SavingsAccounts = savingsAccounts;
            ViewBag.CreditCards = creditCards;

            return View(new SaveTransactionViewModel { UserId = userId });
        }


        [HttpPost]
        public async Task<IActionResult> CreditCardPayment(SaveTransactionViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserId = userId;

            // Obtener la cuenta de origen
            var sourceAccount = await _savingsAccountService.GetByIdAsync(vm.SavingsAccountId.Value);
            if (sourceAccount == null || sourceAccount.Balance < vm.Amount)
            {
                TempData["ErrorMessage"] = "No tienes fondos suficientes para realizar este pago.";
                return RedirectToAction("CreditCardPayment");
            }

            // Obtener la tarjeta de crédito
            var creditCard = await _creditCardService.GetByIdAsync(vm.CreditCardId.Value);
            if (creditCard == null)
            {
                TempData["ErrorMessage"] = "La tarjeta de crédito seleccionada no es válida.";
                return RedirectToAction("CreditCardPayment");
            }

            // Asegurar que el usuario no pague más de la deuda
            decimal amountToPay = Math.Min(vm.Amount, creditCard.Debt);

            if (amountToPay <= 0)
            {
                TempData["ErrorMessage"] = "El monto del pago debe ser mayor a 0.";
                return RedirectToAction("CreditCardPayment");
            }

            // Enviar a la vista de confirmación
            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = creditCard.CardNumber,
                RecipientName = "Pago de Tarjeta de Crédito",
                Amount = amountToPay
            };

            return View("ConfirmCreditCardPayment", confirmationVm);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmCreditCardPayment(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el pago.";
                return View("ConfirmCreditCardPayment", vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transaction = new SaveTransactionViewModel
            {
                UserId = userId,
                Amount = vm.Amount,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.FromAccountNumber),
                CreditCardId = await _creditCardService.GetCardIdByNumberAsync(vm.ToAccountNumber)
            };

            bool success = await _transactionService.ProcessCreditCardPayment(transaction);

            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar el pago.";
                return RedirectToAction("CreditCardPayment");
            }

            TempData["SuccessMessage"] = "Pago realizado con éxito.";
            return RedirectToAction("Client", "Home");
        }

        public IActionResult CancelCreditCardPayment()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("CreditCardPayment");
        }

        #endregion


        #region Pago de Préstamo
        public async Task<IActionResult> LoanPayment()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.SavingsAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            ViewBag.Loans = await _loanService.GetByUserIdAsync(userId);
            return View(new SaveTransactionViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> LoanPayment(SaveTransactionViewModel vm)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserId = userId;

            var sourceAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            var sourceAccount = sourceAccounts.FirstOrDefault(a => a.Id == vm.SavingsAccountId);

            if (sourceAccount == null || sourceAccount.Balance < vm.Amount)
            {
                TempData["ErrorMessage"] = "No tienes fondos suficientes para realizar este pago.";
                return RedirectToAction("LoanPayment");
            }

            var loan = await _loanService.GetByIdAsync(vm.LoanId.Value);
            if (loan == null)
            {
                TempData["ErrorMessage"] = "El préstamo seleccionado no es válido.";
                return RedirectToAction("LoanPayment");
            }

            decimal amountToPay = Math.Min(vm.Amount, loan.RemainingBalance);

            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = $"LOAN-{loan.Id}",
                RecipientName = "Pago de Préstamo",
                Amount = amountToPay
            };

            return View("ConfirmLoanPayment", confirmationVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmLoanPayment(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el pago.";
                return View("ConfirmLoanPayment", vm);
            }

            var transaction = new SaveTransactionViewModel
            {
                UserId = vm.UserId,
                Amount = vm.Amount,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.FromAccountNumber),
                LoanId = int.Parse(vm.ToAccountNumber.Replace("LOAN-", ""))
            };

            bool success = await _transactionService.ProcessLoanPayment(transaction);

            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar el pago.";
                return RedirectToAction("LoanPayment");
            }

            TempData["SuccessMessage"] = "Pago realizado con éxito.";
            return RedirectToAction("Client", "Home");


        }

        public IActionResult CancelLoanPayment()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("LoanPayment");
        }

        #endregion


        #region Pago a Beneficiarios
        public async Task<IActionResult> BeneficiaryPayment()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener los beneficiarios del usuario
            var beneficiaries = await _beneficiaryService.GetByUserIdAsync(userId);

            // Si no tiene beneficiarios, mostrar un mensaje de error y redirigirlo
            if (beneficiaries == null || !beneficiaries.Any())
            {
                TempData["ErrorMessage"] = "No tienes beneficiarios agregados. Agrega al menos uno antes de realizar un pago.";
                return RedirectToAction("Index", "Beneficiary"); // Redirigir a la gestion de beneficiarios
            }

            ViewBag.Beneficiaries = beneficiaries;
            ViewBag.SavingsAccounts = await _savingsAccountService.GetByUserIdAsync(userId);

            return View(new SaveTransactionViewModel { UserId = userId });
        }


        [HttpPost]
        public async Task<IActionResult> BeneficiaryPayment(SaveTransactionViewModel vm)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserId = userId;

            var sourceAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            var sourceAccount = sourceAccounts.FirstOrDefault(a => a.Id == vm.SavingsAccountId);

            if (sourceAccount == null || sourceAccount.Balance < vm.Amount)
            {
                TempData["ErrorMessage"] = "No tienes fondos suficientes para realizar este pago.";
                return RedirectToAction("BeneficiaryPayment");
            }

            var beneficiary = await _beneficiaryService.GetByIdAsync(vm.BeneficiaryId.Value);
            if (beneficiary == null)
            {
                TempData["ErrorMessage"] = "El beneficiario seleccionado no es válido.";
                return RedirectToAction("BeneficiaryPayment");
            }

            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = beneficiary.AccountNumber,
                RecipientName = $"{beneficiary.FullName}",
                Amount = vm.Amount
            };

            return View("ConfirmBeneficiaryPayment", confirmationVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBeneficiaryPayment(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el pago.";

                if (string.IsNullOrEmpty(vm.RecipientName) || string.IsNullOrEmpty(vm.ToAccountNumber))
                {
                    var beneficiary = await _beneficiaryService.GetByIdAsync(vm.BeneficiaryId.Value);
                    if (beneficiary != null)
                    {
                        vm.RecipientName = beneficiary.FullName;
                        vm.ToAccountNumber = beneficiary.AccountNumber;
                    }
                }

                return View("ConfirmBeneficiaryPayment", vm);
            }

            var transaction = new SaveTransactionViewModel
            {
                UserId = vm.UserId,
                Amount = vm.Amount,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.FromAccountNumber),
                BeneficiaryId = vm.BeneficiaryId
            };

            bool success = await _transactionService.ProcessBeneficiaryPayment(transaction);

            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar el pago.";
                return RedirectToAction("BeneficiaryPayment");
            }

            TempData["SuccessMessage"] = "Pago realizado con éxito.";
            return RedirectToAction("Client", "Home");
        }


        public IActionResult CancelPaymentBeneficiary()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("BeneficiaryPayment");
        }

        #endregion


        #region Avance de Efectivo

        public async Task<IActionResult> CashAdvance()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.SavingsAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            ViewBag.CreditCards = await _creditCardService.GetByUserIdAsync(userId);
            return View(new SaveTransactionViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> CashAdvance(SaveTransactionViewModel vm)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserId = userId;

            // Validar la tarjeta de credito seleccionada
            var creditCard = await _creditCardService.GetByIdAsync(vm.CreditCardId.Value);
            if (creditCard == null)
            {
                TempData["ErrorMessage"] = "La tarjeta de crédito seleccionada no es válida.";
                return RedirectToAction("CashAdvance");
            }

            // Validar la cuenta de ahorro seleccionada
            var savingsAccount = await _savingsAccountService.GetByIdAsync(vm.SavingsAccountId.Value);
            if (savingsAccount == null)
            {
                TempData["ErrorMessage"] = "La cuenta de ahorro seleccionada no es válida.";
                return RedirectToAction("CashAdvance");
            }

            // Validar que el avance de efectivo no supere el limite de la tarjeta
            decimal availableCredit = creditCard.CreditLimit - creditCard.Debt;
            if (vm.Amount > availableCredit)
            {
                TempData["ErrorMessage"] = "El monto del avance de efectivo supera el límite disponible en la tarjeta.";
                return RedirectToAction("CashAdvance");
            }

            // Calcular la nueva deuda con el 6.25% de interes
            decimal totalWithInterest = vm.Amount * 1.0625m;

            
            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = creditCard.CardNumber, 
                ToAccountNumber = savingsAccount.AccountNumber, 
                RecipientName = "Avance de Efectivo",
                Amount = vm.Amount
            };

            return View("ConfirmCashAdvance", confirmationVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCashAdvance(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el avance de efectivo.";
                return View("ConfirmCashAdvance", vm);
            }

            var transaction = new SaveTransactionViewModel
            {
                UserId = vm.UserId,
                Amount = vm.Amount,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.ToAccountNumber),
                CreditCardId = await _creditCardService.GetCardIdByNumberAsync(vm.FromAccountNumber)
            };

            bool success = await _transactionService.ProcessCashAdvance(transaction);

            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar el avance de efectivo.";
                return RedirectToAction("CashAdvance");
            }

            TempData["SuccessMessage"] = "Avance de efectivo realizado con éxito.";
            return RedirectToAction("Client", "Home");
        }

        public IActionResult CancelCashAdvance()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("CashAdvance");
        }


        #endregion


        #region Transferencia entre Cuentas Internas

        public async Task<IActionResult> InternalTransfer()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener las cuentas del usuario logueado
            var userAccounts = await _savingsAccountService.GetByUserIdAsync(userId);

            if (userAccounts == null || !userAccounts.Any())
            {
                TempData["ErrorMessage"] = "No tienes cuentas registradas para realizar transferencias.";
                return RedirectToAction("Client", "Home");
            }

            ViewBag.UserAccounts = userAccounts;
            return View(new SaveTransactionViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> InternalTransfer(SaveTransactionViewModel vm)
        {
           
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserId = userId;

            // Obtener la cuenta de origen y destino
            var userAccounts = await _savingsAccountService.GetByUserIdAsync(userId);
            var sourceAccount = userAccounts.FirstOrDefault(a => a.Id == vm.SavingsAccountId);
            var destinationAccount = userAccounts.FirstOrDefault(a => a.Id == vm.ToAccountId);

            if (sourceAccount == null || destinationAccount == null)
            {
                TempData["ErrorMessage"] = "Las cuentas seleccionadas no son válidas.";
                return RedirectToAction("InternalTransfer");
            }

            if (sourceAccount.Balance < vm.Amount)
            {
                TempData["ErrorMessage"] = "No tienes fondos suficientes para realizar esta transferencia.";
                return RedirectToAction("InternalTransfer");
            }

       
            var confirmationVm = new ConfirmTransactionViewModel
            {
                UserId = vm.UserId,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = destinationAccount.AccountNumber,
                RecipientName = "Transferencia entre cuentas propias",
                Amount = vm.Amount
            };

            return View("ConfirmInternalTransfer", confirmationVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmInternalTransfer(ConfirmTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar la transferencia.";
                return View("ConfirmInternalTransfer", vm);
            }

            var transaction = new SaveTransactionViewModel
            {
                UserId = vm.UserId,
                Amount = vm.Amount,
                SavingsAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.FromAccountNumber),
                ToAccountId = await _savingsAccountService.GetAccountIdByNumberAsync(vm.ToAccountNumber)
            };

            bool success = await _transactionService.ProcessInternalTransfer(transaction);

            if (!success)
            {
                TempData["ErrorMessage"] = "No se pudo procesar la transferencia.";
                return RedirectToAction("InternalTransfer");
            }

            TempData["SuccessMessage"] = "Transferencia realizada con éxito.";
            return RedirectToAction("Client", "Home");
        }

        public IActionResult CancelInternalTransfer()
        {
            TempData["InfoMessage"] = "El pago ha sido cancelado.";
            return RedirectToAction("InternalTransfer");
        }


        #endregion


    }
}
