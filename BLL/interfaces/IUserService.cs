using DAL.Models;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<object> GetUsers(DataSourceLoadOptions loadOptions);
        Task<bool> IsUsernameExists(string username);
    }
}
