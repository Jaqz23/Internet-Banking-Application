namespace IB.Core.Application.ViewModels.Product
{
    public class ProductsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProductType { get; set; } // "Cuenta de ahorro", "Tarjeta de credito", "Prestamo"
        public string AccountNumber { get; set; } // Numero unico del producto
        public decimal Balance { get; set; } // Saldo actual
        public decimal Debt { get; set; } // Deuda (solo para tarjetas de credito y prestamos)
        public bool IsPrimary { get; set; } // Indica si la cuenta de ahorro es principal
    }
}
