using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Dealer> Dealers { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Employee> Employees { get; }
        Task<int> SaveChangesAsync();
        Task ResetIdentityAsync(string tableName);
    }
}
