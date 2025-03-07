using DAL.Exceptions;
using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Employee;

namespace SystemManager.Managers.Employee
{
    public class EmployeeManager : BaseManager, IEmployeeManager
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeManager(
            IRepositoryManager repositoryManager,
            IEmployeeService employeeService,
            ILogger<EmployeeManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _employeeService = employeeService;
        }

        public async Task<object> GetEmployees(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Employees List");
            return await _employeeService.GetEmployees(loadOptions);
        }

        public async Task<bool> IsTcNoExists(string tcNo)
        {
            await LogActionAsync("Check TC No Exists", $"TCNo: {tcNo}");

            if (string.IsNullOrEmpty(tcNo))
            {
                throw new ValidationException("TC No cannot be empty");
            }

            return await _employeeService.IsTcNoExists(tcNo);
        }

        public async Task<object> GetTechnicians(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Technicians List");
            return await _employeeService.GetTechnicians(loadOptions);
        }

        public async Task<EmployeeModel> GetByIdAsync(int id)
        {
            await LogActionAsync("Get Employee By Id", $"EmployeeId: {id}");
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {id} not found");
            }

            return employee;
        }

        public async Task CreateAsync(EmployeeModel employee)
        {
            await LogActionAsync("Create Employee", $"Name: {employee.Name}");

            if (string.IsNullOrEmpty(employee.Name))
            {
                throw new ValidationException("Employee name cannot be empty");
            }

            if (!string.IsNullOrEmpty(employee.TCIDNO) && await IsTcNoExists(employee.TCIDNO))
            {
                throw new ValidationException($"TC No {employee.TCIDNO} already exists");
            }

            var count = await _employeeService.GetRepository().GetQueryable().CountAsync();
            var lastEmployee = await _employeeService.GetRepository().GetQueryable()
                .OrderByDescending(s => s.ID)
                .FirstOrDefaultAsync();

            employee.DepartmentID = 3;
            employee.TitleID = 2;
            employee.CrmID = Guid.NewGuid();
            employee.IsActive = true;
            employee.HourlyLaborCost = 42;
            employee.RowGuid = Guid.NewGuid();
            employee.IsRegionManager = false;
            employee.UserCode = $"UC0{count + 1}";
            employee.CreatedOn = DateTime.UtcNow;
            employee.CreatedBy = CurrentUserService.GetCurrentUserId();
            employee.ModifiedBy = null;
            employee.ModifiedOn = DateTime.UtcNow;

            await _employeeService.CreateAsync(employee);
        }

        public async Task<IActionResult> UpdateAsync(EmployeeModel employee)
        {
            await LogActionAsync("Update Employee", $"EmployeeId: {employee.ID}");

            if (string.IsNullOrEmpty(employee.Name))
            {
                throw new ValidationException("Employee name cannot be empty");
            }

            var existingEmployee = await GetByIdAsync(employee.ID);

            if (!string.IsNullOrEmpty(employee.TCIDNO) &&
                employee.TCIDNO != existingEmployee.TCIDNO &&
                await IsTcNoExists(employee.TCIDNO))
            {
                throw new ValidationException($"TC No {employee.TCIDNO} already exists");
            }

            employee.ModifiedOn = DateTime.UtcNow;
            employee.ModifiedBy = CurrentUserService.GetCurrentUserId();
            employee.CreatedOn = existingEmployee.CreatedOn;
            employee.CreatedBy = existingEmployee.CreatedBy;
            employee.RowGuid = existingEmployee.RowGuid;

            await _employeeService.UpdateAsync(employee);
            return new OkObjectResult(new { success = true, message = "Employee updated successfully" });
        }

        public async Task DeleteAsync(int id)
        {
            await LogActionAsync("Delete Employee", $"EmployeeId: {id}");

            var employee = await GetByIdAsync(id);

            if (employee.IsActive == true)
            {
                throw new BusinessException("Active employees cannot be deleted");
            }

            await _employeeService.DeleteAsync(id);
            await RepositoryManager.SaveChangesAsync();
            await RepositoryManager.ResetIdentityAsync("Employee");
        }
    }
}
