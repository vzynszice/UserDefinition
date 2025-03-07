using DAL.Models;
using DAL.Repositories;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository Properties
        IGenericRepository<UserModel> Users { get; }
        IGenericRepository<DealerModel> Dealers { get; }
        IGenericRepository<ServiceModel> Services { get; }
        IGenericRepository<EmployeeModel> Employees { get; }
        IGenericRepository<CarModel> Cars { get; }
        IGenericRepository<UserEmployee> UserEmployees { get; }
        IGenericRepository<Title> Titles { get; }
        IGenericRepository<Language> Languages { get; }
        IGenericRepository<MenuItem> MenuItems { get; }
        IGenericRepository<PartDamage> PartDamages { get; }

        // Transaction management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task ResetIdentityAsync(string tableName);
    }
}
