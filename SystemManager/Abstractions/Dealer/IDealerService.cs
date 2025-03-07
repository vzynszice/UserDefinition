using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Common;

namespace SystemManager.Abstractions.Dealer 
{
    public interface IDealerService : IGenericService<DealerModel>
    {
        Task<object> GetDealers(DataSourceLoadOptions loadOptions);
        Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions);
        IGenericRepository<DealerModel> GetRepository();
    }
}
