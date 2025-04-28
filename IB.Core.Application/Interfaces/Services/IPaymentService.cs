namespace IB.Core.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<int> GetPaymentsMadeAllTheTime();
        Task<int> GetPaymentsMadeLast24Hours();
    }
}
