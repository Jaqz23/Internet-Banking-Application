namespace IB.Core.Application.ViewModels.Transaction
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? SavingsAccountId { get; set; }
        public int? CreditCardId { get; set; }
        public int? LoanId { get; set; }
        public int? BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string FromUserName { get; set; }  // Nombre del usuario que envía
        public string ToUserName { get; set; }    // Nombre del usuario que recibe

        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
    }

}
