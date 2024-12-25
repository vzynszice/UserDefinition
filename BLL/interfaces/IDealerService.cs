using DAL.Models;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.interfaces
{
    public interface IDealerService : IGenericService<Dealer>
    {
        Task<object> GetDealers(DataSourceLoadOptions loadOptions);
        Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions);
    }
}
