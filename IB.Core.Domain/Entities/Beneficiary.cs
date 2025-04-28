using IB.Core.Domain.Common;

namespace IB.Core.Domain.Entities
{
    public class Beneficiary : BaseEntity
    {
        public string UserId { get; set; }
        public int SavingsAccountId { get; set; } 
        public string BeneficiaryName { get; set; }

        // Navigation Properties
        public SavingsAccount SavingsAccount { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
