using DAL.Models;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.interfaces
{
    public interface IEmployeeService : IGenericService<Employee>
    {
        Task<bool> IsTcNoExists(string tcNo);
        Task<object> GetEmployees(DataSourceLoadOptions loadOptions);
        Task<object> GetTechnicians(DataSourceLoadOptions loadOptions);
    }
}
