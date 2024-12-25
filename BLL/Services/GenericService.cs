using BLL.interfaces;
using DAL.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        protected abstract IGenericRepository<T> GetRepository();
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetRepository().GetAllAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await GetRepository().GetByIdAsync(id);
        }
        public virtual async Task CreateAsync(T entity)
        {
            await GetRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await GetRepository().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            await GetRepository().DeleteAsync(id);
        }

    }
}
