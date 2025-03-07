using DAL.Exceptions;
using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.User;

namespace BLL.Services
{
    public class UserService : GenericService<UserModel>, IUserService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override IGenericRepository<UserModel> GetRepository()
        {
            return _repositoryManager.GetRepository<UserModel>();
        }

        public async Task<object> GetUsers(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
            {
                throw new ValidationException("LoadOptions cannot be null");
            }

            var users = GetRepository().GetQueryable()
                .Select(u => new
                {
                    u.ID,
                    u.Username,
                    u.Email,
                    u.Name,
                    u.Surname,
                    u.DealerID,
                    u.ServiceID,
                    u.IsLockedOut
                });

            return await DataSourceLoader.LoadAsync(users, loadOptions);
        }

        public async Task<bool> IsUsernameExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            return await GetRepository().GetQueryable()
                .AnyAsync(e => e.Username.ToLower() == username.ToLower());
        }
        public async Task<bool> IsUsernameExistsExcept(string username, int userId)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ValidationException("Username cannot be empty");
            }

            return await GetRepository().GetQueryable()
                .Where(u => u.ID != userId)
                .AnyAsync(e => e.Username.ToLower() == username.ToLower());
        }
    }
}