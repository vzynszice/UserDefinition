using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Auth;
using SystemManager.Abstractions.Car;
using SystemManager.Abstractions.Dealer;
using SystemManager.Abstractions.Employee;
using SystemManager.Abstractions.Service;
using SystemManager.Abstractions.User;
namespace SystemManager.Abstractions
{
    public interface ISystemManager
    {
        IRepositoryManager RepositoryManager { get; }
        ICarDamageManager CarDamageManager { get; }
        IDealerManager DealerManager { get; }
        IEmployeeManager EmployeeManager { get; }
        IServiceManager ServiceManager { get; }
        IUserManager UserManager { get; }
    }
}
