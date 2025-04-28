using IB.Core.Domain.Common;

namespace IB.Core.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; } // Monto total del prestamo
        public decimal RemainingBalance { get; set; } // Saldo pendiente

        // Navigation Properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
