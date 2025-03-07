using DAL.Models;
using DevExtreme.AspNet.Mvc;

namespace SystemManager.Abstractions.Service
{
    public interface IServiceManager
    {
        Task<object> GetServices(DataSourceLoadOptions loadOptions);
        Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions);
        Task<ServiceModel> GetByIdAsync(int id);
        Task CreateAsync(ServiceModel service);
        Task UpdateAsync(ServiceModel service);
        Task DeleteAsync(int id);
    }
}
