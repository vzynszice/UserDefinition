using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace SystemManager.Abstractions.User
{
    public interface IUserManager
    {
        Task<object> GetUsers(DataSourceLoadOptions loadOptions);
        Task<bool> IsUsernameExists(string username);
        Task<bool> IsUsernameExistsExcept(string username, int userId);
        Task<UserModel> GetByIdAsync(int id);
        Task CreateAsync(UserModel user);
        Task<IActionResult> UpdateAsync(UserModel user);
        Task DeleteAsync(int id);
    }
}
