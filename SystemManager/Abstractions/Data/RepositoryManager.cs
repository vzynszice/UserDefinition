using DAL.Models;
using DAL.Repositories;
using DAL.UnitOfWork;
using Microsoft.Extensions.Logging;


namespace SystemManager.Abstractions.Data
{
    public class RepositoryManager : IRepositoryManager // BaseManager'dan miras almıyoruz
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RepositoryManager> _logger;

        public RepositoryManager(
            IUnitOfWork unitOfWork,
            ILogger<RepositoryManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            Type entityType = typeof(T);

            if (entityType == typeof(CarModel))
                return (IGenericRepository<T>)_unitOfWork.Cars;
            if (entityType == typeof(DealerModel))
                return (IGenericRepository<T>)_unitOfWork.Dealers;
            if (entityType == typeof(ServiceModel))
                return (IGenericRepository<T>)_unitOfWork.Services;
            if (entityType == typeof(EmployeeModel))
                return (IGenericRepository<T>)_unitOfWork.Employees;
            if (entityType == typeof(UserModel))
                return (IGenericRepository<T>)_unitOfWork.Users;

            throw new NotSupportedException($"Repository for type {entityType.Name} is not supported");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task ResetIdentityAsync(string tableName)
        {
            await _unitOfWork.ResetIdentityAsync(tableName);
        }
    }
}