using IB.Core.Application.ViewModels.Product;
using IB.Core.Application.ViewModels.SavingsAccount;

namespace IB.Core.Application.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; }
        public bool HasError { get; set; }
        public string? Error { get; set; }

        // Lista de productos asignados al usuario
        public List<ProductsViewModel> Products { get; set; } = new List<ProductsViewModel>();
       
        // Cuentas de ahorro del usuario
        public List<SavingsAccountViewModel> SavingsAccounts { get; set; } = new();
    }
}
