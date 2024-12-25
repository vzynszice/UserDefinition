using DAL.Models;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.interfaces
{
    public interface IServiceService : IGenericService<Service>
    {
        Task<object> GetServices(DataSourceLoadOptions loadOptions);
        Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions);
    }
}
