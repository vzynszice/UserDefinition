using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Common;

namespace SystemManager.Abstractions.User
{
    public interface IUserService : IGenericService<UserModel>
    {
        Task<object> GetUsers(DataSourceLoadOptions loadOptions);
        Task<bool> IsUsernameExists(string username);
        Task<bool> IsUsernameExistsExcept(string username, int userId);
        IGenericRepository<UserModel> GetRepository();
    }
}
