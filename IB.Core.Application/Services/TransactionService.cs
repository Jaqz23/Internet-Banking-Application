using AutoMapper;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.Transaction;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Services
{
    public class TransactionService : GenericService<SaveTransactionViewModel, TransactionViewModel, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ISavingsAccountRepository savingsAccountRepository,
            ICreditCardRepository creditCardRepository,
            ILoanRepository loanRepository,
            IBeneficiaryRepository beneficiaryRepository,
            IMapper mapper
        ) : base(transactionRepository, mapper)
        {
            _transactionRepository = transactionRepository;
            _savingsAccountRepository = savingsAccountRepository;
            _creditCardRepository = creditCardRepository;
            _loanRepository = loanRepository;
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper;
        }

        public async Task<List<TransactionViewModel>> GetAllTransactionsByUserId(string userId)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<List<TransactionViewModel>>(transactions);
        }


        // Pago Expreso
        public async Task<bool> ProcessExpressPayment(SaveTransactionViewModel transaction)
        {
            var sourceAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            var destinationAccount = await _savingsAccountRepository.GetByAccountNumberAsync(transaction.ToAccountNumber!);

            if (sourceAccount == null || destinationAccount == null || sourceAccount.Balance < transaction.Amount)
                return false;

            // Realizar la transaccion
            sourceAccount.Balance -= transaction.Amount;
            destinationAccount.Balance += transaction.Amount;

            // Guardar los cambios en las cuentas
            await _savingsAccountRepository.UpdateAsync(sourceAccount, sourceAccount.Id);
            await _savingsAccountRepository.UpdateAsync(destinationAccount, destinationAccount.Id);

            // Registrar la transaccion
            var newTransaction = _mapper.Map<Transaction>(transaction);
            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


        // Pago de Tarjeta de Credito
        public async Task<bool> ProcessCreditCardPayment(SaveTransactionViewModel transaction)
        {
            if (transaction.CreditCardId == null || transaction.SavingsAccountId == null)
                return false; 

            var sourceAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            var creditCard = await _creditCardRepository.GetByIdAsync(transaction.CreditCardId.Value);

            if (sourceAccount == null || creditCard == null || sourceAccount.Balance < transaction.Amount)
                return false;

            decimal amountToPay = Math.Min(transaction.Amount, creditCard.Debt);
            creditCard.Debt -= amountToPay;
            sourceAccount.Balance -= amountToPay;

            await _savingsAccountRepository.UpdateAsync(sourceAccount, sourceAccount.Id);
            await _creditCardRepository.UpdateAsync(creditCard, creditCard.Id);

            var newTransaction = new Transaction
            {
                UserId = transaction.UserId,
                SavingsAccountId = transaction.SavingsAccountId,
                CreditCardId = transaction.CreditCardId,
                Amount = amountToPay,
                Date = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


        // Pago de Prestamo
        public async Task<bool> ProcessLoanPayment(SaveTransactionViewModel transaction)
        {
            var sourceAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            var loan = await _loanRepository.GetByIdAsync(transaction.LoanId.Value);

            if (sourceAccount == null || loan == null || sourceAccount.Balance < transaction.Amount)
                return false;

            // Calcular el monto real que se pagara (el menor entre la deuda y el monto ingresado)
            decimal amountToPay = Math.Min(transaction.Amount, loan.RemainingBalance);

            // Restar el monto de la cuenta de origen
            sourceAccount.Balance -= amountToPay;

            // Restar el monto del prestamo
            loan.RemainingBalance -= amountToPay;

            // Guardar cambios
            await _savingsAccountRepository.UpdateAsync(sourceAccount, sourceAccount.Id);
            await _loanRepository.UpdateAsync(loan, loan.Id);

            // Registrar la transaccion asegurando que el monto sea correcto
            var newTransaction = _mapper.Map<Transaction>(transaction);
            newTransaction.Amount = amountToPay;
            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


        // Pago a Beneficiario
        public async Task<bool> ProcessBeneficiaryPayment(SaveTransactionViewModel transaction)
        {
            var sourceAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            var beneficiary = await _beneficiaryRepository.GetByIdWithAccountAsync(transaction.BeneficiaryId.Value);

            if (sourceAccount == null || beneficiary == null || beneficiary.SavingsAccount == null)
                return false;

            var destinationAccount = beneficiary.SavingsAccount;

            if (sourceAccount.Balance < transaction.Amount)
                return false;

            // Realizar la transaccion
            sourceAccount.Balance -= transaction.Amount;
            destinationAccount.Balance += transaction.Amount;

            await _savingsAccountRepository.UpdateAsync(sourceAccount, sourceAccount.Id);
            await _savingsAccountRepository.UpdateAsync(destinationAccount, destinationAccount.Id);

            // Registrar la transaccion
            var newTransaction = _mapper.Map<Transaction>(transaction);
            newTransaction.BeneficiaryId = beneficiary.Id;
            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


        public async Task<int> GetTransactionsMadeAllTheTime()
        {
            return await _transactionRepository.GetTotalTransactions();
        }

        public async Task<int> GetTransactionsMadeLast24Hours()
        {
            return await _transactionRepository.GetTransactionsLast24Hours();
        }


        public async Task<bool> ProcessCashAdvance(SaveTransactionViewModel transaction)
        {
            // Validar tarjeta de credito
            var creditCard = await _creditCardRepository.GetByIdAsync(transaction.CreditCardId.Value);
            if (creditCard == null)
                return false;

            // Validar cuenta de ahorro
            var savingsAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            if (savingsAccount == null)
                return false;

            // Verificar que el avance de efectivo no supere el limite de credito disponible
            decimal availableCredit = creditCard.CreditLimit - creditCard.Debt;

            if (transaction.Amount > availableCredit) // Monto original sin interes
                return false;

            //  Calcular la nueva deuda con el 6.25% de interes
            decimal totalWithInterest = transaction.Amount * 1.0625m;

            
            savingsAccount.Balance += transaction.Amount;
            creditCard.Debt += totalWithInterest;

          
            await _savingsAccountRepository.UpdateAsync(savingsAccount, savingsAccount.Id);
            await _creditCardRepository.UpdateAsync(creditCard, creditCard.Id);

           
            var newTransaction = _mapper.Map<Transaction>(transaction);
            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


        public async Task<bool> ProcessInternalTransfer(SaveTransactionViewModel transaction)
        {
            // Validar cuenta de origen
            var sourceAccount = await _savingsAccountRepository.GetByIdAsync(transaction.SavingsAccountId.Value);
            if (sourceAccount == null || sourceAccount.Balance < transaction.Amount)
                return false;

            // Validar cuenta de destino
            var destinationAccount = await _savingsAccountRepository.GetByIdAsync(transaction.ToAccountId.Value);
            if (destinationAccount == null)
                return false;

            // Verificar que la cuenta de origen y destino no sean la misma
            if (sourceAccount.Id == destinationAccount.Id)
                return false;

            // Realizar la transferencia
            sourceAccount.Balance -= transaction.Amount;
            destinationAccount.Balance += transaction.Amount;

            // Guardar cambios
            await _savingsAccountRepository.UpdateAsync(sourceAccount, sourceAccount.Id);
            await _savingsAccountRepository.UpdateAsync(destinationAccount, destinationAccount.Id);

            // Registrar la transaccion
            var newTransaction = _mapper.Map<Transaction>(transaction);
            await _transactionRepository.AddAsync(newTransaction);

            return true;
        }


    }

}
