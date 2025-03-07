
using DAL.Models;

namespace SystemManager.Abstractions.Auth
{
    public interface IAuthenticationManager
    {
        Task<AuthResult> LoginAsync(string username, string password);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<bool> IsUserActiveAsync(string username);
    }
}
