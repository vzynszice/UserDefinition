using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Common;

namespace SystemManager.Abstractions.Service    
{
    public interface IServiceService : IGenericService<ServiceModel>
    {
        Task<object> GetServices(DataSourceLoadOptions loadOptions);
        Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions);
        IGenericRepository<ServiceModel> GetRepository();
    }
}
