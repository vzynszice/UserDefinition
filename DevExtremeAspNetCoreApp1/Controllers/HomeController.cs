using BLL.interfaces;
using DAL.db;
using DAL.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DevExtremeAspNetCoreApp1.Exceptions;
using DevExtremeAspNetCoreApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DevExtremeAspNetCoreApp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDealerService _dealerService;
        private readonly IServiceService _serviceService;
        private readonly IEmployeeService _employeeService;
        

        public HomeController(IUserService userService, IDealerService dealerService, IServiceService serviceService, IEmployeeService employeeService)
        {
            _userService = userService;
            _dealerService = dealerService;
            _serviceService = serviceService;
            _employeeService = employeeService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(DataSourceLoadOptions loadOptions)
        {
            if(loadOptions == null)
                throw new ValidationException("loadOptions is null");
            var users = await _userService.GetUsers(loadOptions);
            return Json(users);
            
        }

        [HttpGet]
        public async Task<IActionResult> GetDealers(DataSourceLoadOptions loadOptions)
        {
            if(loadOptions==null)
                throw new ValidationException("loadOptions is null");
            var dealers = await _dealerService.GetDealers(loadOptions);
            return Json(dealers);
        }

        [HttpGet]
        public async Task<IActionResult> GetServices(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
                throw new ValidationException("loadOptions is null");
            var services = await _serviceService.GetServices(loadOptions);
            return Json(services);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
                throw new ValidationException("loadOptions is null");
            var employess = await _employeeService.GetEmployees(loadOptions);
            return Json(employess);
        }

        [HttpGet]
        public async Task<IActionResult> GetDealersForUsersAndServices(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
                throw new ValidationException("loadOptions is null");
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request parameters");

            var result = await _dealerService.GetDealersForDropdown(loadOptions);

            if (result==null)
                throw new ValidationException("Invalid response data");
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetServicesByID(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
                throw new ValidationException("loadOptions is null");
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request parameters");

            var result = await _serviceService.GetServicesForDropdown(loadOptions);

            if (result == null)
                throw new ValidationException("Invalid response data");
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetTechnicians(DataSourceLoadOptions loadOptions)
        {
            if (loadOptions == null)
                throw new ValidationException("loadOptions is null");
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request parameters");

            var result = await _employeeService.GetTechnicians(loadOptions);

            if (result == null)
                throw new ValidationException("Invalid response data");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string values)
        {
            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No data received");

            if (!ModelState.IsValid)
                throw new ValidationException("Invalid user data");

            var user = new User();
            JsonConvert.PopulateObject(values, user);

            if (string.IsNullOrEmpty(user.Username))
                throw new ValidationException("Username is required");

            if (string.IsNullOrEmpty(user.Email))
                throw new ValidationException("Email is required");

            var updates = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);
            if (updates.ContainsKey("username"))
            {
                string username = updates["username"].ToString();
                if (!string.IsNullOrEmpty(username) && await _userService.IsUsernameExists(username))
                    throw new ValidationException($"Bu username {username} sistemde zaten kayıtlı");
            }

            await _userService.CreateAsync(user);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddDealer(string values)
        {
            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No data received");
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid dealer data");

            var dealer = new Dealer();
            JsonConvert.PopulateObject(values, dealer);

            await _dealerService.CreateAsync(dealer);
            return Ok(dealer);
        }
        [HttpPost]
        public async Task<IActionResult> AddService(string values)
        {
            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No data received");
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid dealer data");

            var service = new Service();
            JsonConvert.PopulateObject(values, service);

            await _serviceService.CreateAsync(service);
            return Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(string values)
        {
            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No data received");

            var updates = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);

            // TC Kimlik kontrolü
            if (updates.ContainsKey("tcidno"))
            {
                string tcNo = updates["tcidno"].ToString();
                
                if (!string.IsNullOrEmpty(tcNo) && await _employeeService.IsTcNoExists(tcNo))
                    throw new ValidationException($"Bu TC Kimlik Numarası ({tcNo}) sistemde zaten kayıtlı.");
               
                if (tcNo.Length!= 11)
                {
                    throw new ValidationException("TC Kimlik Numarası 11 haneli olmalıdır");
                }
            }
            int serviceID = Int32.Parse(updates["serviceID"].ToString());
            if (serviceID == 0)
            {
                throw new ValidationException("ServiceID girilmelidir");
            }

            var employee = new Employee();
            MapEmployeeFromUpdates(updates, employee);

            // Zorunlu alan kontrolü
            if (employee.DealerID == 0)
                throw new ValidationException("DealerID is required");

            await _employeeService.CreateAsync(employee);
            return Ok(employee);
        }

        private void MapEmployeeFromUpdates(Dictionary<string, object> updates, Employee employee)
        {
            if (updates.ContainsKey("name"))
                employee.Name = updates["name"].ToString();
            if (updates.ContainsKey("dealerID"))
                employee.DealerID = Convert.ToInt32(updates["dealerID"]);
            if (updates.ContainsKey("serviceID"))
                employee.ServiceID = updates["serviceID"] == null ? null : Convert.ToInt32(updates["serviceID"]);
            if (updates.ContainsKey("email"))
                employee.Email = updates["email"].ToString();
            if (updates.ContainsKey("tcidno"))
                employee.TCIDNO = updates["tcidno"].ToString();
            if (updates.ContainsKey("isTechnician"))
            {
                employee.IsTechnician = Convert.ToBoolean(updates["isTechnician"]);
            }
            else
            {
                employee.IsTechnician = false;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int key)
        {
            if (key <= 0)
                throw new ValidationException("Invalid user id");

            var user = await _userService.GetByIdAsync(key);
            if (user == null)
                throw new NotFoundException("User not found");

            await _userService.DeleteAsync(key);
            return NoContent(); // 204 status code - DevExtreme için ideal
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDealer(int key)
        {
            if (key <= 0)
                throw new ValidationException("Invalid dealer id");

            var dealer = await _dealerService.GetByIdAsync(key);
            if (dealer == null)
                throw new NotFoundException("Dealer not found");

            await _dealerService.DeleteAsync(key);
            return Ok(new { success = true }); // DevExtreme için özel yanıt formatı
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteService(int key)
        {
            if (key <= 0)
                throw new ValidationException("Invalid service id");

            var service = await _serviceService.GetByIdAsync(key);
            if (service == null)
                throw new NotFoundException("Service not found");

            await _serviceService.DeleteAsync(key);
            return Ok(new { success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int key)
        {
            if (key <= 0)
                throw new ValidationException("Invalid employee id");

            var employee = await _employeeService.GetByIdAsync(key);
            if (employee == null)
                throw new NotFoundException("Employee not found");

            await _employeeService.DeleteAsync(key);
            return Ok(new { success = true });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int key, string values)
        {
            if (key <= 0)
                throw new ValidationException("Invalid user id");

            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No update data received");

            var user = await _userService.GetByIdAsync(key);
            if (user == null)
                throw new NotFoundException($"User with ID {key} not found");

            JsonConvert.PopulateObject(values, user);

            if (string.IsNullOrEmpty(user.Username))
                throw new ValidationException("Username cannot be empty");

            if (string.IsNullOrEmpty(user.Email))
                throw new ValidationException("Email cannot be empty");

            await _userService.UpdateAsync(user);
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDealer(int key, string values)
        {
            if (key <= 0)
                throw new ValidationException("Invalid dealer id");

            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No update data received");

            var dealer = await _dealerService.GetByIdAsync(key);
            if (dealer == null)
                throw new NotFoundException($"Dealer with ID {key} not found");

            JsonConvert.PopulateObject(values, dealer);
            dealer.HostName = Environment.MachineName;

            await _dealerService.UpdateAsync(dealer);
            return Ok(dealer);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService(int key, string values)
        {
            if (key <= 0)
                throw new ValidationException("Invalid service id");

            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No update data received");

            var service = await _serviceService.GetByIdAsync(key);
            if (service == null)
                throw new NotFoundException($"Service with ID {key} not found");

            JsonConvert.PopulateObject(values, service);

            await _serviceService.UpdateAsync(service);
            return Ok(service);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(int key, string values)
        {
            if (key <= 0)
                throw new ValidationException("Invalid employee id");

            if (string.IsNullOrEmpty(values))
                throw new ValidationException("No update data received");

            var employee = await _employeeService.GetByIdAsync(key);
            if (employee == null)
                throw new NotFoundException($"Employee with ID {key} not found");

            var updates = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);

            if (updates.ContainsKey("name"))
                employee.Name = updates["name"].ToString();

            if (updates.ContainsKey("dealerID"))
                employee.DealerID = Convert.ToInt32(updates["dealerID"]);

            if (updates.ContainsKey("serviceID"))
                employee.ServiceID = updates["serviceID"] == null ? null : Convert.ToInt32(updates["serviceID"]);

            if (updates.ContainsKey("email"))
                employee.Email = updates["email"].ToString();

            if (updates.ContainsKey("tcidno"))
                employee.TCIDNO = updates["tcidno"].ToString();

            if (updates.ContainsKey("isTechnician"))
                employee.IsTechnician = Convert.ToBoolean(updates["isTechnician"]);

            await _employeeService.UpdateAsync(employee);
            return Ok(employee);
        }
    }
}