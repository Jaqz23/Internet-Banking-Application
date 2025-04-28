using IB.Core.Domain.Common;

namespace IB.Core.Domain.Entities
{
    public class CreditCard : BaseEntity
    {
        public string UserId { get; set; }
        public string CardNumber { get; set; } // Numero unico de tarjeta
        public decimal CreditLimit { get; set; } // Limite de credito
        public decimal Debt { get; set; } = 0; // Monto adeudado

        // Navigation Properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
