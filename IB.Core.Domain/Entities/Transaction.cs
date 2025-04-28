using IB.Core.Domain.Common;

namespace IB.Core.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string UserId { get; set; }
        public int? SavingsAccountId { get; set; }
        public int? CreditCardId { get; set; }
        public int? LoanId { get; set; }
        public int? BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public SavingsAccount? SavingsAccount { get; set; }
        public CreditCard? CreditCard { get; set; }
        public Loan? Loan { get; set; }
        public Beneficiary? Beneficiary { get; set; }

    }
}
