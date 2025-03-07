using DAL.Exceptions;
using Microsoft.Extensions.Logging;
using SystemManager.Abstractions.User;
using SystemManager.Abstractions.Common;
using DevExtreme.AspNet.Mvc;
using DAL.Models;
using SystemManager.Abstractions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SystemManager.Managers.User
{
    public class UserManager : BaseManager, IUserManager
    {
        private readonly IUserService _userService;

        public UserManager(
            IRepositoryManager repositoryManager,
            IUserService userService,
            ILogger<UserManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _userService = userService;
        }

        public async Task<object> GetUsers(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Users List");
            return await _userService.GetUsers(loadOptions);
        }

        public async Task<bool> IsUsernameExists(string username)
        {
            await LogActionAsync("Check Username Exists", $"Username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            return await _userService.IsUsernameExists(username);
        }

        public async Task<UserModel> GetByIdAsync(int id)
        {
            await LogActionAsync("Get User By Id", $"UserId: {id}");
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found");
            }

            return user;
        }

        public async Task CreateAsync(UserModel user)
        {
            await LogActionAsync("Create User", $"Username: {user.Username}");

            if (string.IsNullOrEmpty(user.Username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            if (await IsUsernameExists(user.Username))
            {
                throw new ValidationException($"Username {user.Username} already exists");
            }

            user.CreatedOn = DateTime.Now;
            user.RowGuid = Guid.NewGuid();
            user.Active = 1;
            user.IsApproved = true;
            user.IsLockedOut = false;
            user.FailedPasswordAttemptCount = 0;
            user.FailedPasswordAnswerAttemptCount = 0;
            user.DoNotApplyCrmRole = false;
            user.IsSAPUser = 0;
            user.IsUserForSAP = false;
            user.IsUserForDMSWeb = true;
            user.IsRequiredMfa = false;
            user.IsPasswordTemporary = true;
            user.PasswordExpireDate = DateTime.Now.AddDays(90);
            user.CreatedBy = CurrentUserService.GetCurrentUserId();

            await _userService.CreateAsync(user);
        }

        public async Task<IActionResult> UpdateAsync(UserModel user)
        {
            try
            {
                await LogActionAsync("Update User", $"UserId: {user.ID}");

                // Username boş mu kontrol et
                if (string.IsNullOrEmpty(user.Username))
                {
                    return new BadRequestObjectResult(new { success = false, message = "Username cannot be empty" });
                }

                // Mevcut kullanıcıyı bul
                var existingUser = await GetByIdAsync(user.ID);
                if (existingUser == null)
                {
                    return new NotFoundObjectResult(new { success = false, message = $"User with ID {user.ID} not found" });
                }

                // Kullanıcı adı değiştirilmiş mi kontrol et
                if (!user.Username.Equals(existingUser.Username, StringComparison.OrdinalIgnoreCase))
                {
                    // Yeni username başka bir kullanıcıda var mı kontrol et
                    if (await _userService.IsUsernameExistsExcept(user.Username, user.ID))
                    {
                        return new ConflictObjectResult(new { success = false, message = $"Username '{user.Username}' is already taken by another user" });
                    }
                }

                // Güncelleme işlemleri
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = CurrentUserService.GetCurrentUserId();
                user.CreatedOn = existingUser.CreatedOn;
                user.CreatedBy = existingUser.CreatedBy;
                user.RowGuid = existingUser.RowGuid;
                user.IsUserForDMSWeb = true;

                await _userService.UpdateAsync(user);

                return new OkObjectResult(new { success = true, message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                // Genel hata durumunda loglama yapabilirsiniz
                Console.Error.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await LogActionAsync("Delete User", $"UserId: {id}");

            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found");
            }

            await _userService.DeleteAsync(id);
            await RepositoryManager.SaveChangesAsync();
            await RepositoryManager.ResetIdentityAsync("User");
        }
        public async Task<bool> IsUsernameExistsExcept(string username, int userId)
        {
            await LogActionAsync("Check Username Exists Except", $"Username: {username}, UserId: {userId}");

            if (string.IsNullOrEmpty(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            return await _userService.IsUsernameExistsExcept(username, userId);
        }
    }
}
