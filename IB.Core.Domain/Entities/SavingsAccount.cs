using IB.Core.Domain.Common;

namespace IB.Core.Domain.Entities
{
    public class SavingsAccount : BaseEntity
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; } // Numero unico de 9 dígitos
        public decimal Balance { get; set; } = 0;
        public bool IsPrimary { get; set; } = false; // Solo una cuenta es la principal

        // Navigation Properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    }
}
