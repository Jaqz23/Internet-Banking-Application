using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;

namespace IB.Core.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionRepository _transactionRepository;

        public PaymentService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<int> GetPaymentsMadeAllTheTime()
        {
            return await _transactionRepository.GetTotalPayments();
        }

        public async Task<int> GetPaymentsMadeLast24Hours()
        {
            return await _transactionRepository.GetPaymentsLast24Hours();
        }
    }

}
