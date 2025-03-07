using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Dealer;
namespace BLL.Services
{
    public class DealerService : GenericService<DealerModel>, IDealerService
    {
        private readonly IRepositoryManager _repositoryManager;

        // Constructor now accepts IRepositoryManager
        public DealerService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override IGenericRepository<DealerModel> GetRepository()
        {
            return _repositoryManager.GetRepository<DealerModel>();
        }

        public async Task<object> GetDealers(DataSourceLoadOptions loadOptions)
        {
            var dealers = GetRepository().GetQueryable()
                .Select(d => new
                {
                    d.ID,
                    d.Name,
                    d.PhoneNumber,
                    d.WebAddress,
                    d.Email,
                    d.OpeningDate,
                    d.AddressLine1,
                    d.PostalCode
                });

            return await DataSourceLoader.LoadAsync(dealers, loadOptions);
        }

        public async Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions)
        {
            var dealers = GetRepository().GetQueryable()
                .Select(d => new
                {
                    d.ID,
                    d.Name
                });

            return await DataSourceLoader.LoadAsync(dealers, loadOptions);
        }
    }

}
