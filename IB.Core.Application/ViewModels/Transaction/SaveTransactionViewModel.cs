using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.Transaction
{
    public class SaveTransactionViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string UserId { get; set; }

        public int? SavingsAccountId { get; set; }
        public int? CreditCardId { get; set; }
        public int? LoanId { get; set; }
        public int? BeneficiaryId { get; set; }
        public int? ToAccountId { get; set; } // Para transferencias internas


        [StringLength(9, MinimumLength = 9, ErrorMessage = "El número de cuenta debe tener exactamente 9 dígitos.")]
        public string? ToAccountNumber { get; set; }


        [Required(ErrorMessage = "Debe ingresar un monto válido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0.")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public bool HasError { get; set; }
        public string? Error { get; set; }


        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        public string TransactionType { get; set; }  // "Express", "CreditCard", "Loan", "Beneficiary"



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((TransactionType == "Express" || TransactionType == "Beneficiary") && string.IsNullOrEmpty(ToAccountNumber))
            {
                yield return new ValidationResult("Debe ingresar el número de cuenta de destino.", new[] { nameof(ToAccountNumber) });
            }

            if (TransactionType == "CashAdvance")
            {
                if (!CreditCardId.HasValue)
                    yield return new ValidationResult("Debe seleccionar una tarjeta de crédito para el avance.", new[] { nameof(CreditCardId) });

                if (!SavingsAccountId.HasValue)
                    yield return new ValidationResult("Debe seleccionar una cuenta de ahorro para depositar el avance.", new[] { nameof(SavingsAccountId) });
            }

            if (TransactionType == "InternalTransfer")
            {
                if (!SavingsAccountId.HasValue)
                    yield return new ValidationResult("Debe seleccionar la cuenta de origen.", new[] { nameof(SavingsAccountId) });

                if (!ToAccountId.HasValue)
                    yield return new ValidationResult("Debe seleccionar la cuenta de destino.", new[] { nameof(ToAccountId) });

                if (SavingsAccountId == ToAccountId)
                    yield return new ValidationResult("No puedes transferir a la misma cuenta.", new[] { nameof(ToAccountId) });
            }
        }
    }

}
