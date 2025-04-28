using IB.Core.Application.Dtos.Account;
using IB.Core.Application.ViewModels.User;

namespace IB.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task ActivateUserAsync(string id);
        Task<List<UserViewModel>> GetAllAsync();
        Task<SaveUserViewModel> GetByIdAsync(string id);
        Task InactivateUserAsync(string id);
        Task<AuthenticationResponse> LoginAsync(LoginViewModel vm);
        Task<RegisterResponse> RegisterAsync(SaveUserViewModel vm, string origin);
        Task SignOutAsync();
        Task UpdateAsync(SaveUserViewModel vm, string id);
    }
}
