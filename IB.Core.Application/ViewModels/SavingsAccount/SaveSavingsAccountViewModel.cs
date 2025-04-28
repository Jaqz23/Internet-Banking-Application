using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.SavingsAccount
{
    public class SaveSavingsAccountViewModel
    {
        public int? Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
        public string AccountNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El balance no puede ser negativo.")]
        public decimal Balance { get; set; }

        public bool IsPrimary { get; set; }
    }
}
