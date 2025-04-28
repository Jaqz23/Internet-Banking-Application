namespace IB.Core.Application.ViewModels.SavingsAccount
{
    public class SavingsAccountViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsPrimary { get; set; }
    }
}
