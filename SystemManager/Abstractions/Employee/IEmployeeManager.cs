using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace SystemManager.Abstractions.Employee
{
    public interface IEmployeeManager
    {
        Task<object> GetEmployees(DataSourceLoadOptions loadOptions);
        Task<bool> IsTcNoExists(string tcNo);
        Task<object> GetTechnicians(DataSourceLoadOptions loadOptions);
        Task<EmployeeModel> GetByIdAsync(int id);
        Task CreateAsync(EmployeeModel employee);
        Task<IActionResult> UpdateAsync(EmployeeModel employee);
        Task DeleteAsync(int id);
    }
}
