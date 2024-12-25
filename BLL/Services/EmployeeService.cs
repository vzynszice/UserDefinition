using BLL.interfaces;
using DAL.db;
using DAL.interfaces;
using DAL.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {

        public EmployeeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override IGenericRepository<Employee> GetRepository()
        {
            return _unitOfWork.Employees;
        }

        public async Task<object> GetEmployees(DataSourceLoadOptions loadOptions)
        {
            var employees = GetRepository().GetQueryable().Select(s => new
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
            return await GetRepository().GetQueryable().AnyAsync(e => e.TCIDNO == tcNo);
        }
        public async Task<object> GetTechnicians(DataSourceLoadOptions loadOptions)
        {
            var technicians = GetRepository().GetQueryable()
                .Where(e => (bool)e.IsTechnician)
                .Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    isTechnician = s.IsTechnician
                });

            return await DataSourceLoader.LoadAsync(technicians, loadOptions);
        }

        public override async Task CreateAsync(Employee entity)
        {
            if (!string.IsNullOrEmpty(entity.TCIDNO) &&
                await GetRepository().GetQueryable().AnyAsync(e => e.TCIDNO == entity.TCIDNO))
            {
                throw new InvalidOperationException($"Bu TC Kimlik Numarası ({entity.TCIDNO}) sistemde zaten kayıtlı.");
            }
            var count = GetRepository().GetQueryable().Count();
            var lastEmployee = GetRepository().GetQueryable()
                .OrderByDescending(s => s.ID)
                .FirstOrDefault();
            entity.DepartmentID = 3;
            entity.TitleID = 2;
            entity.CrmID = Guid.NewGuid();
            entity.IsActive = true;
            entity.HourlyLaborCost = 42;
            entity.RowGuid = Guid.NewGuid();
            entity.IsRegionManager = false;
            entity.UserCode = $"UC0{count + 1}";
            entity.CreatedOn = DateTime.UtcNow;
            entity.CreatedBy = null;
            entity.ModifiedBy = null;
            entity.ModifiedOn = DateTime.UtcNow;
            entity.CreatedOn = DateTime.UtcNow;

            await base.CreateAsync(entity);
        }
        public override async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.ResetIdentityAsync("Employee");
        }
    }
}
