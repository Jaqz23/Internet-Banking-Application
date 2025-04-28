using System.ComponentModel.DataAnnotations;

namespace IB.Core.Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un nombre de usuario")]
        [Display(Name = "User name")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
       
        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
