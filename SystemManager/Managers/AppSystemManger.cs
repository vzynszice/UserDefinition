using SystemManager.Abstractions;
using SystemManager.Abstractions.Auth;
using SystemManager.Abstractions.Car;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Dealer;
using SystemManager.Abstractions.Employee;
using SystemManager.Abstractions.Service;
using SystemManager.Abstractions.User;

namespace SystemManager.Managers
{
    public class AppSystemManager : ISystemManager
    {
        public IRepositoryManager RepositoryManager { get; }
        public ICarDamageManager CarDamageManager { get; }
        public IDealerManager DealerManager { get; }
        public IEmployeeManager EmployeeManager { get; }
        public IServiceManager ServiceManager { get; }
        public IUserManager UserManager { get; }

        public AppSystemManager(
            IRepositoryManager repositoryManager,
            ICarDamageManager carDamageManager,
            IDealerManager dealerManager,
            IEmployeeManager employeeManager,
            IServiceManager serviceManager,
            IUserManager userManager)
        {
            RepositoryManager = repositoryManager;
            CarDamageManager = carDamageManager;
            DealerManager = dealerManager;
            EmployeeManager = employeeManager;
            ServiceManager = serviceManager;
            UserManager = userManager;
        }
    }
}
