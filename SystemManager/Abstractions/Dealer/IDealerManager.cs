using DAL.Models;
using DevExtreme.AspNet.Mvc;

namespace SystemManager.Abstractions.Dealer
{
    public interface IDealerManager
    {
        Task<object> GetDealers(DataSourceLoadOptions loadOptions);
        Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions);
        Task<DealerModel> GetByIdAsync(int id);
        Task CreateAsync(DealerModel dealer);
        Task UpdateAsync(DealerModel dealer);
        Task DeleteAsync(int id);
    }
}
