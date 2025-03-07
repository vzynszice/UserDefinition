using DAL.Repositories;

namespace SystemManager.Abstractions.Data
{
    public interface IRepositoryManager
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task ResetIdentityAsync(string tableName);
    }
}
