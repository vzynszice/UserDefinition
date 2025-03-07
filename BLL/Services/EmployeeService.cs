using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Employee;

namespace BLL.Services
{
    public class EmployeeService : GenericService<EmployeeModel>, IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;

        // Constructor now accepts IRepositoryManager
        public EmployeeService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override IGenericRepository<EmployeeModel> GetRepository()
        {
            return _repositoryManager.GetRepository<EmployeeModel>();
        }
        public async Task<object> GetEmployees(DataSourceLoadOptions loadOptions)
        {
            var employees = GetRepository().GetQueryable()
                .Select(s => new
                {
                    s.ID,
                    s.Name,
                    s.DealerID,
                    s.ServiceID,
                    s.Email,
                    s.TCIDNO,
                    s.IsTechnician,
                    s.CreatedOn
                });

            return await DataSourceLoader.LoadAsync(employees, loadOptions);
        }

        public async Task<bool> IsTcNoExists(string tcNo)
        {
            return await GetRepository().GetQueryable()
                .AnyAsync(e => e.TCIDNO == tcNo);
        }

        public async Task<object> GetTechnicians(DataSourceLoadOptions loadOptions)
        {
            var technicians = GetRepository().GetQueryable()
                .Where(e => e.IsTechnician == true)
                .Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    isTechnician = s.IsTechnician
                });

            return await DataSourceLoader.LoadAsync(technicians, loadOptions);
        }
    }
}
