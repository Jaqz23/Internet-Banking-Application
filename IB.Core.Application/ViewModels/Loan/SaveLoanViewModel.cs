using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.Loan
{
    public class SaveLoanViewModel
    {
        public int? Id { get; set; }
        public string UserId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto del préstamo debe ser positivo.")]
        public decimal Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El saldo restante no puede ser negativo.")]
        public decimal RemainingBalance { get; set; }
    }
}
