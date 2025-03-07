using DAL.Exceptions;
using DAL.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;

namespace BLL.Services
{
    // BLL/Services/GenericService.cs
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IRepositoryManager _repositoryManager;

        // Constructor now accepts IRepositoryManager instead of ISystemManager
        protected GenericService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        // GetRepository method retrieves the repository directly from IRepositoryManager
        public virtual IGenericRepository<T> GetRepository()
        {
            return _repositoryManager.GetRepository<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetRepository().GetAllAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await GetRepository().GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Entity of type {typeof(T).Name} with id {id} was not found");
            }
            return entity;
        }

        public virtual async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ValidationException($"Entity of type {typeof(T).Name} cannot be null");
            }
            await GetRepository().AddAsync(entity);
            await _repositoryManager.SaveChangesAsync(); // Save changes directly via IRepositoryManager
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ValidationException($"Entity of type {typeof(T).Name} cannot be null");
            }
            await GetRepository().UpdateAsync(entity);
            await _repositoryManager.SaveChangesAsync(); // Save changes directly via IRepositoryManager
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetRepository().GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Entity of type {typeof(T).Name} with id {id} was not found");
            }
            await GetRepository().DeleteAsync(id);
            await _repositoryManager.SaveChangesAsync(); // Save changes directly via IRepositoryManager
        }
    }
}