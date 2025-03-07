using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Service;

namespace BLL.Services
{
    public class ServiceService : GenericService<ServiceModel>, IServiceService
    {
        private readonly IRepositoryManager _repositoryManager;

        // Constructor now accepts IRepositoryManager
        public ServiceService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override IGenericRepository<ServiceModel> GetRepository()
        {
            return _repositoryManager.GetRepository<ServiceModel>();
        }

        public async Task<object> GetServices(DataSourceLoadOptions loadOptions)
        {
            var services = GetRepository().GetQueryable()
                .Select(s => new
                {
                    s.ID,
                    s.Name,
                    s.PhoneNumber,
                    s.MapUrl,
                    s.Email,
                    s.OpeningDate,
                    s.AddressLine1,
                    s.PostalCode,
                    s.DealerID
                });

            return await DataSourceLoader.LoadAsync(services, loadOptions);
        }

        public async Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions)
        {
            var services = GetRepository().GetQueryable()
                .Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    dealerID = s.DealerID
                });

            return await DataSourceLoader.LoadAsync(services, loadOptions);
        }
    }
}
