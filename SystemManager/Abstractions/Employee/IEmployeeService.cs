using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Common;

namespace SystemManager.Abstractions.Employee
{
    public interface IEmployeeService : IGenericService<EmployeeModel>
    {
        Task<object> GetEmployees(DataSourceLoadOptions loadOptions);
        Task<bool> IsTcNoExists(string tcNo);
        Task<object> GetTechnicians(DataSourceLoadOptions loadOptions);
        IGenericRepository<EmployeeModel> GetRepository();
    }
}
