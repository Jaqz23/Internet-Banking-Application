namespace IB.Core.Application.ViewModels.Loan
{
    public class LoanViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}
