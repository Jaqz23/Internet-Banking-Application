using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.Product
{
    using System.ComponentModel.DataAnnotations;

    public class SaveProductViewModel : IValidatableObject
    {
        public int? Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de producto.")]
        [Display(Name = "Tipo de Producto")]
        public string ProductType { get; set; } // SavingAccount, CreditCard, Loan

        [Display(Name = "Número de Cuenta/Tarjeta")]
        public string? AccountNumber { get; set; }

        [Display(Name = "Saldo Inicial")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo.")]
        public decimal Balance { get; set; } = 0;

        [Display(Name = "Límite de Tarjeta")]
        public decimal? CreditLimit { get; set; } // Solo para tarjetas de credito

        [Display(Name = "Monto del Préstamo")]
        public decimal? Amount { get; set; } // Solo para prestamos

        [Display(Name = "Deuda")]
        public decimal Debt { get; set; } = 0; // Solo para tarjetas de credito y prestamos

        [Display(Name = "¿Es cuenta principal?")]
        public bool IsPrimary { get; set; } = false; // Solo para cuentas de ahorro

        public bool HasError { get; set; }
        public string? Error { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductType == "CreditCard" && (CreditLimit == null || CreditLimit <= 0))
            {
                yield return new ValidationResult("El límite de crédito debe ser mayor a 0.", new[] { nameof(CreditLimit) });
            }

            if (ProductType == "Loan" && (Amount == null || Amount <= 0))
            {
                yield return new ValidationResult("El monto del préstamo debe ser mayor a 0.", new[] { nameof(Amount) });
            }
        }
    }

}
