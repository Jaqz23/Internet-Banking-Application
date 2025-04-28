using IB.Core.Application.ViewModels.Transaction;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Interfaces.Services
{
    public interface ITransactionService : IGenericService<SaveTransactionViewModel, TransactionViewModel, Transaction>
    {
        Task<List<TransactionViewModel>> GetAllTransactionsByUserId(string userId);
        Task<bool> ProcessExpressPayment(SaveTransactionViewModel transaction);
        Task<bool> ProcessCreditCardPayment(SaveTransactionViewModel transaction);
        Task<bool> ProcessLoanPayment(SaveTransactionViewModel transaction);
        Task<bool> ProcessBeneficiaryPayment(SaveTransactionViewModel transaction);
        Task<bool> ProcessCashAdvance(SaveTransactionViewModel transaction);
        Task<bool> ProcessInternalTransfer(SaveTransactionViewModel transaction);


        Task<int> GetTransactionsMadeAllTheTime();
        Task<int> GetTransactionsMadeLast24Hours();
    }

}
