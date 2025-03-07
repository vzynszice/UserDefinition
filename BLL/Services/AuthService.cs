using DAL.Models;
using Microsoft.EntityFrameworkCore;
using SystemManager.Abstractions.Auth;
using SystemManager.Abstractions.Data;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepositoryManager _repositoryManager;

        public AuthService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _repositoryManager
                .GetRepository<UserModel>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    ErrorMessage = "User not found."
                };
            }
            if (user.Password != password)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid password."
                };
            }
            return new AuthResult
            {
                Succeeded = true,
                User = user
            };
        }
    }
}