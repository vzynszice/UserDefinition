using DAL.Models;
using DevExtreme.AspNet.Mvc;
using DAL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SystemManager.Abstractions.User;
using SystemManager.Abstractions.Dealer;
using SystemManager.Abstractions.Employee;
using SystemManager.Abstractions.Service;
using Microsoft.EntityFrameworkCore;


namespace DevExtremeAspNetCoreApp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IDealerManager _dealerManager;
        private readonly IServiceManager _serviceManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IUserManager userManager,
            IDealerManager dealerManager,
            IServiceManager serviceManager,
            IEmployeeManager employeeManager,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _dealerManager = dealerManager;
            _serviceManager = serviceManager;
            _employeeManager = employeeManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Get Methods

        [HttpGet]
        public async Task<IActionResult> GetUsers(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var users = await _userManager.GetUsers(loadOptions);
                return Json(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching users" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userManager.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = $"User with ID {id} not found" });
                }

                return Json(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID {id}");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching the user data" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDealers(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var dealers = await _dealerManager.GetDealers(loadOptions);
                return Json(dealers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dealers");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching dealers" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServices(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var services = await _serviceManager.GetServices(loadOptions);
                return Json(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching services" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var employees = await _employeeManager.GetEmployees(loadOptions);
                return Json(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching employees" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDealersForUsersAndServices(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var result = await _dealerManager.GetDealersForDropdown(loadOptions);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dealers for dropdown");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching dealers for dropdown" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServicesByID(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var result = await _serviceManager.GetServicesForDropdown(loadOptions);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services for dropdown");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching services for dropdown" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTechnicians(DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (loadOptions == null)
                    return BadRequest(new { success = false, message = "loadOptions is null" });

                var result = await _employeeManager.GetTechnicians(loadOptions);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching technicians");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching technicians" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username, int? userId)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new { success = false, message = "Username cannot be empty" });
                }

                bool exists;
                if (userId.HasValue)
                {
                    // Mevcut kullanıcı için kontrol (update durumu)
                    exists = await _userManager.IsUsernameExistsExcept(username, userId.Value);
                }
                else
                {
                    // Yeni kullanıcı için kontrol (create durumu)
                    exists = await _userManager.IsUsernameExists(username);
                }

                if (exists)
                {
                    return Json(new { success = false, message = $"Username '{username}' already exists" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking username '{username}'");
                return StatusCode(500, new { success = false, message = "An error occurred while checking username" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidateTCIDNO(string tcidno, int? currentId)
        {
            try
            {
                // Validate format first
                if (string.IsNullOrEmpty(tcidno) || !System.Text.RegularExpressions.Regex.IsMatch(tcidno, @"^\d{11}$"))
                {
                    return Json(new { isValid = false, message = "TC Kimlik No 11 haneli rakamlardan oluşmalıdır" });
                }

                // If we're updating (currentId has value), get the original employee
                bool isTcNoExists = false;
                if (currentId.HasValue)
                {
                    var originalEmployee = await _employeeManager.GetByIdAsync(currentId.Value);

                    // Only check for duplicate if TCIDNO has changed
                    if (originalEmployee != null && tcidno != originalEmployee.TCIDNO)
                    {
                        isTcNoExists = await _employeeManager.IsTcNoExists(tcidno);
                    }
                }
                else
                {
                    // For new records, directly check if it exists
                    isTcNoExists = await _employeeManager.IsTcNoExists(tcidno);
                }

                return Json(new { isValid = !isTcNoExists, message = isTcNoExists ? "Bu TC Kimlik No sistemde zaten var" : "" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating TCIDNO");
                return Json(new { isValid = false, message = "Doğrulama sırasında hata oluştu" });
            }
        }


        #endregion

        #region Add Methods

        [HttpPost]
        public async Task<IActionResult> AddUser(string values)
        {
            try
            {
                _logger.LogInformation($"AddUser called with values: {values}");

                if (string.IsNullOrEmpty(values))
                    return BadRequest(new { success = false, message = "No data received" });

                var user = new UserModel();
                JsonConvert.PopulateObject(values, user);

                // Server-side validation
                if (string.IsNullOrEmpty(user.Username))
                    return BadRequest(new { success = false, message = "Username is required" });

                if (string.IsNullOrEmpty(user.Password))
                    return BadRequest(new { success = false, message = "Password is required" });

                if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Surname))
                    return BadRequest(new { success = false, message = "Name and surname are required" });

                // Generate email in the required format
                user.Email = $"{user.Name.ToLower().Replace(" ", "")}{user.Surname.ToLower().Replace(" ", "")}@hyundai.com.tr";

                // Check if username already exists
                if (await _userManager.IsUsernameExists(user.Username))
                    return Conflict(new { success = false, message = "Username already exists" });

                await _userManager.CreateAsync(user);
                return Ok(new { success = true, message = "User created successfully", user });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while adding user");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while adding user");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user");
                return StatusCode(500, new { success = false, message = "An error occurred while adding the user" });
            }
        }

        [HttpPost]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        public async Task<IActionResult> AddDealer([FromForm] string values)
        {
            try
            {
                _logger.LogInformation($"AddDealer called with values: {values}");

                if (string.IsNullOrEmpty(values))
                    return BadRequest(new { success = false, message = "No data received" });

                var dealer = new DealerModel();
                JsonConvert.PopulateObject(values, dealer);

                await _dealerManager.CreateAsync(dealer);
                return Ok(new { success = true, message = "Dealer created successfully", dealer });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while adding dealer");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while adding dealer");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding dealer");
                return StatusCode(500, new { success = false, message = "An error occurred while adding the dealer" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddService([FromForm] string values)
        {
            try
            {
                _logger.LogInformation($"AddService called with values: {values}");

                if (string.IsNullOrEmpty(values))
                    return BadRequest(new { success = false, message = "No data received" });

                var service = new ServiceModel();
                JsonConvert.PopulateObject(values, service);

                await _serviceManager.CreateAsync(service);
                return Ok(new { success = true, message = "Service created successfully", service });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding service");
                return StatusCode(500, new { success = false, message = "An error occurred while adding the service" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeModel employee)
        {
            try
            {
                _logger.LogInformation($"AddEmployee called with data: {JsonConvert.SerializeObject(employee)}");

                if (employee == null)
                    return BadRequest(new { success = false, message = "No data received" });

                // Validate TCIDNO format
                if (string.IsNullOrEmpty(employee.TCIDNO) || !System.Text.RegularExpressions.Regex.IsMatch(employee.TCIDNO, @"^\d{11}$"))
                {
                    return BadRequest(new { success = false, message = "TC Kimlik No 11 haneli rakamlardan oluşmalıdır" });
                }

                // Let the employee manager handle the TCIDNO validation and creation
                // Your EmployeeManager.CreateAsync already checks for duplicate TCIDNO
                await _employeeManager.CreateAsync(employee);
                return Ok(new { success = true, message = "Employee created successfully", employee });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while adding employee");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while adding employee");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while adding employee");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee");
                return StatusCode(500, new { success = false, message = "An error occurred while adding the employee" });
            }
        }


        #endregion

        #region Delete Methods

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            // Try to get the key from form data
            int? key = null;

            // Read form data for DELETE requests
            if (Request.HasFormContentType && Request.Form.ContainsKey("key"))
            {
                if (int.TryParse(Request.Form["key"], out int formKey))
                {
                    key = formKey;
                }
            }

            if (!key.HasValue)
            {
                return BadRequest(new { success = false, message = "User ID is required" });
            }

            await _userManager.DeleteAsync(key.Value);
            return Ok(new { success = true, message = "User deleted successfully" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDealer(int key)
        {
            try
            {
                await _dealerManager.DeleteAsync(key);
                return Ok(new { success = true, message = "Dealer deleted successfully" });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, $"Business rule violation while deleting dealer {key}");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting dealer with ID {key}");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the dealer" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteService(int key)
        {
            try
            {
                await _serviceManager.DeleteAsync(key);
                return Ok(new { success = true, message = "Service deleted successfully" });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, $"Business rule violation while deleting service {key}");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service with ID {key}");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the service" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int key)
        {
            try
            {
                await _employeeManager.DeleteAsync(key);
                return Ok(new { success = true, message = "Employee deleted successfully" });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, $"Business rule violation while deleting employee {key}");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {key}");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the employee" });
            }
        }

        #endregion

        #region Update Methods

        [HttpPut]
        public async Task<IActionResult> UpdateUser(int key, [FromForm] string values)
        {
            try
            {
                _logger.LogInformation($"UpdateUser called with key: {key}, values: {values}");

                if (string.IsNullOrEmpty(values))
                {
                    _logger.LogWarning("No values data received in UpdateUser");
                    return BadRequest(new { success = false, message = "No data received" });
                }

                // Get the original user
                var originalUser = await _userManager.GetByIdAsync(key);
                if (originalUser == null)
                {
                    _logger.LogWarning($"User with ID {key} not found in UpdateUser");
                    return NotFound(new { success = false, message = $"User with ID {key} not found" });
                }

                // Store original username and password for comparison
                var originalUsername = originalUser.Username;
                var originalPassword = originalUser.Password;

                // Apply the updated values to the original user object
                JsonConvert.PopulateObject(values, originalUser);

                // Server-side validation
                if (string.IsNullOrEmpty(originalUser.Username))
                {
                    return BadRequest(new { success = false, message = "Username is required" });
                }

                if (string.IsNullOrEmpty(originalUser.Name) || string.IsNullOrEmpty(originalUser.Surname))
                {
                    return BadRequest(new { success = false, message = "Name and surname are required" });
                }

                // Generate email based on current name and surname
                originalUser.Email = $"{originalUser.Name.ToLower().Replace(" ", "")}{originalUser.Surname.ToLower().Replace(" ", "")}@hyundai.com.tr";

                // Check if username changed and if it's unique
                if (originalUser.Username != originalUsername && await _userManager.IsUsernameExistsExcept(originalUser.Username, key))
                {
                    return Conflict(new { success = false, message = "Username already exists" });
                }

                // If password is empty, keep the original password
                if (string.IsNullOrEmpty(originalUser.Password))
                {
                    originalUser.Password = originalPassword;
                }

                // Update the user
                await _userManager.UpdateAsync(originalUser);

                return Ok(new { success = true, message = "User updated successfully" });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON parsing error while updating user with ID {key}: {ex.Message}");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, $"Business rule violation while updating user with ID {key}: {ex.Message}");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with ID {key}: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the user" });
            }
        }

        [HttpPut]
        [Route("/Home/UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeModel employee)
        {
            try
            {
                _logger.LogInformation($"UpdateEmployee called with employee data: {JsonConvert.SerializeObject(employee)}");

                // Validate employee
                if (employee == null)
                {
                    return BadRequest(new { success = false, message = "No employee data received" });
                }

                if (employee.ID <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid employee ID" });
                }

                // Let the employee manager handle the validation and update
                // Your EmployeeManager.UpdateAsync already checks for duplicate TCIDNO
                var result = await _employeeManager.UpdateAsync(employee);

                // Return the result from the manager
                return result;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating employee");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating employee");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Employee not found while updating");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateEmployee: {ex.Message}");
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDealer(int key, [FromBody] object request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "No update data received" });
                }

                // values parametresini çıkar
                string values = null;
                try
                {
                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.ToString());
                    if (requestDict != null && requestDict.ContainsKey("values"))
                    {
                        values = requestDict["values"];
                    }
                }
                catch
                {
                    // Direkt olarak DealerModel olabilir, o nedenle request'i values olarak kullan
                    values = request.ToString();
                }

                if (string.IsNullOrEmpty(values))
                {
                    return BadRequest(new { success = false, message = "No values data received" });
                }

                var dealer = await _dealerManager.GetByIdAsync(key);
                if (dealer == null)
                {
                    return NotFound(new { success = false, message = $"Dealer with ID {key} not found" });
                }

                JsonConvert.PopulateObject(values, dealer);

                await _dealerManager.UpdateAsync(dealer);

                return Ok(new { success = true, message = "Dealer updated successfully", dealer });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while updating dealer");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating dealer");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating dealer with ID {key}");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the dealer" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService(int key, [FromBody] object request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "No update data received" });
                }

                // values parametresini çıkar
                string values = null;
                try
                {
                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.ToString());
                    if (requestDict != null && requestDict.ContainsKey("values"))
                    {
                        values = requestDict["values"];
                    }
                }
                catch
                {
                    // Direkt olarak ServiceModel olabilir, o nedenle request'i values olarak kullan
                    values = request.ToString();
                }

                if (string.IsNullOrEmpty(values))
                {
                    return BadRequest(new { success = false, message = "No values data received" });
                }

                var service = await _serviceManager.GetByIdAsync(key);
                if (service == null)
                {
                    return NotFound(new { success = false, message = $"Service with ID {key} not found" });
                }

                JsonConvert.PopulateObject(values, service);

                await _serviceManager.UpdateAsync(service);

                return Ok(new { success = true, message = "Service updated successfully", service });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while updating service");
                return BadRequest(new { success = false, message = "Invalid data format: " + ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating service");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating service with ID {key}");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the service" });
            }
        }

        #endregion
    }
}