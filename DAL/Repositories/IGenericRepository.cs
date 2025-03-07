using System.Linq.Expressions;

namespace DAL.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Mevcut metodlar
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        IQueryable<T> GetQueryable();
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
