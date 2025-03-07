using DAL.Exceptions;
using DAL.Models;
using Microsoft.Extensions.Logging;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Auth;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;

namespace SystemManager.Managers.Auth
{
    public class AuthenticationManager : BaseManager, IAuthenticationManager
    {
        private readonly IAuthService _authService;

        public AuthenticationManager(
            IRepositoryManager repositoryManager,
            IAuthService authService,
            ILogger<AuthenticationManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            await LogActionAsync("Login Attempt", $"Username: {username}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ValidationException("Username and password are required");
            }

            if (!await IsUserActiveAsync(username))
            {
                throw new ValidationException("User account is not active");
            }

            var result = await _authService.LoginAsync(username, password);

            if (!result.Succeeded)
            {
                throw new ValidationException(result.ErrorMessage);
            }

            return result;
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            await LogActionAsync("Get User By Username", $"Username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            var user = await _authService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                throw new NotFoundException($"User with username {username} not found");
            }

            return user;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            await LogActionAsync("Validate User Credentials", $"Username: {username}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ValidationException("Username and password are required");
            }

            var result = await _authService.LoginAsync(username, password);
            return result.Succeeded;
        }

        public async Task<bool> IsUserActiveAsync(string username)
        {
            await LogActionAsync("Check User Active Status", $"Username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            var user = await GetUserByUsernameAsync(username);
            return user.Active == 1;
        }
    }
}
