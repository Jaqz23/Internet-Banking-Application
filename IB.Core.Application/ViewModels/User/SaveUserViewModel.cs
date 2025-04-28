using IB.Core.Application.Enums;
using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.User
{
    public class SaveUserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre.")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Debe ingresar el apellido.")]
        [Display(Name = "Apellido")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Debe ingresar un correo electrónico.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar la cédula.")]
        [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "Formato de cédula inválido. Ejemplo: 001-1234567-2")]
        [Display(Name = "Cédula")]
        [DataType(DataType.Text)]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Debe ingresar un nombre de usuario.")]
        [Display(Name = "User name")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario.")]
        [Display(Name = "Tipo de usuario")]
        public Roles UserType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto inicial no puede ser negativo.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto inicial")]
        public double? InitialAmount { get; set; }

        public bool IsActive { get; set; } = true;
        public bool HasError { get; set; }
        public string? Error { get; set; }

        // Para aumentar el saldo de la cuenta principal al editar un cliente
        [Display(Name = "Monto adicional")]
        public double? AdditionalAmount { get; set; }
    }
}
