using DAL.Models;

namespace SystemManager.Abstractions.Auth   
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password);
        Task<UserModel> GetUserByUsernameAsync(string username);
    }

}
