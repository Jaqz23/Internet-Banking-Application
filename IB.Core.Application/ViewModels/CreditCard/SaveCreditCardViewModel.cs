using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.CreditCard
{
    public class SaveCreditCardViewModel
    {
        public int? Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "El número de tarjeta es obligatorio.")]
        public string CardNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El límite de crédito debe ser positivo.")]
        public decimal CreditLimit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La deuda no puede ser negativa.")]
        public decimal Debt { get; set; }
    }
}
