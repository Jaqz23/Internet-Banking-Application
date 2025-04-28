using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.Transaction
{
    public class ConfirmTransactionViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Desde")]
        public string FromAccountNumber { get; set; }

        [Display(Name = "Hacia")]
        public string ToAccountNumber { get; set; }

        [Display(Name = "Destinatario")]
        public string RecipientName { get; set; } = string.Empty;

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        public int? BeneficiaryId { get; set; }
    }

}
