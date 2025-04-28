using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.Beneficiary
{
    public class SaveBeneficiaryViewModel
    {
        public int? Id { get; set; }
        public string? UserOwnerId { get; set; }

        [Required(ErrorMessage = "El número de cuenta es requerido.")]
        [Display(Name = "Número de cuenta")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El número de cuenta debe tener exactamente 9 dígitos.")]
        public string AccountNumberBeneficiary { get; set; }

        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
