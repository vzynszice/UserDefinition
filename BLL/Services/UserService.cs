using BLL.interfaces;
using DAL.db;
using DAL.interfaces;
using DAL.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        public UserService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
            : base(unitOfWork)
        {
            _currentUserService=currentUserService;
        }
        protected override IGenericRepository<User> GetRepository()
        {
            return _unitOfWork.Users;
        }

        public async Task<object> GetUsers(DataSourceLoadOptions loadOptions)
        {
            var users=GetRepository().GetQueryable()
                          .Select(u => new
                          {
                              u.ID,
                              u.Username,
                              u.Email,
                              u.Name,
                              u.Surname,
                              u.Password,
                              u.DealerID,
                              u.ServiceID
                          });
            return await DataSourceLoader.LoadAsync(users, loadOptions);
        }
        public async Task<bool> IsUsernameExists(string username)
        {
            return await GetRepository().GetQueryable().AnyAsync(e => e.Username.Equals(username));
        }

        public override async Task CreateAsync(User entity)
        {
            entity.CreatedOn = DateTime.Now;
            entity.RowGuid = Guid.NewGuid();
            entity.Active = 1;
            entity.IsApproved = true;
            entity.IsLockedOut = false;
            entity.FailedPasswordAttemptCount = 0;
            entity.FailedPasswordAnswerAttemptCount = 0;
            entity.DoNotApplyCrmRole = false;
            entity.IsSAPUser = 0;
            entity.IsUserForSAP = false;
            entity.IsUserForDMSWeb = true;
            entity.IsRequiredMfa = false;
            entity.IsPasswordTemporary = true;
            entity.PasswordExpireDate = DateTime.Now.AddDays(90);

            await base.CreateAsync(entity);
        }
        public override async Task UpdateAsync(User entity)
        {
            var existingUser = await GetRepository().GetByIdAsync(entity.ID);
            if (existingUser == null)
                throw new Exception("User not found");

            var originalCreatedOn = existingUser.CreatedOn;
            var originalRowGuid = existingUser.RowGuid;

            entity.CreatedOn = originalCreatedOn;
            entity.RowGuid = originalRowGuid;
            entity.IsUserForDMSWeb = true;
            entity.ModifiedOn = DateTime.Now;
            entity.ModifiedBy=_currentUserService.GetCurrentUserId();

            await base.UpdateAsync(entity);
        }
        public override async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.ResetIdentityAsync("User");
        }
    }

}
