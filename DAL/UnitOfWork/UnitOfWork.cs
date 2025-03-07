using DAL.Models;
using DAL.Repositories;
using DAL.db;
using Microsoft.EntityFrameworkCore;
using DAL.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HaosDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        // Repository fields
        private IGenericRepository<UserModel> _users;
        private IGenericRepository<DealerModel> _dealers;
        private IGenericRepository<ServiceModel> _services;
        private IGenericRepository<EmployeeModel> _employees;
        private IGenericRepository<CarModel> _cars;
        private IGenericRepository<UserEmployee> _userEmployees;
        private IGenericRepository<Title> _titles;
        private IGenericRepository<Language> _languages;
        private IGenericRepository<MenuItem> _menuItems;
        private IGenericRepository<PartDamage> _partDamages;

        public UnitOfWork(HaosDbContext context)
        {
            _context = context;
        }

        // Repository Properties with lazy loading
        public IGenericRepository<UserModel> Users =>
            _users ??= new GenericRepository<UserModel>(_context);

        public IGenericRepository<DealerModel> Dealers =>
            _dealers ??= new GenericRepository<DealerModel>(_context);

        public IGenericRepository<ServiceModel> Services =>
            _services ??= new GenericRepository<ServiceModel>(_context);

        public IGenericRepository<EmployeeModel> Employees =>
            _employees ??= new GenericRepository<EmployeeModel>(_context);

        public IGenericRepository<CarModel> Cars =>
            _cars ??= new GenericRepository<CarModel>(_context);

        public IGenericRepository<UserEmployee> UserEmployees =>
            _userEmployees ??= new GenericRepository<UserEmployee>(_context);

        public IGenericRepository<Title> Titles =>
            _titles ??= new GenericRepository<Title>(_context);

        public IGenericRepository<Language> Languages =>
            _languages ??= new GenericRepository<Language>(_context);

        public IGenericRepository<MenuItem> MenuItems =>
            _menuItems ??= new GenericRepository<MenuItem>(_context);

        public IGenericRepository<PartDamage> PartDamages =>
            _partDamages ??= new GenericRepository<PartDamage>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new BusinessException("A transaction is already in progress");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task ResetIdentityAsync(string tableName)
        {
            await _context.Database.ExecuteSqlRawAsync(
                $"DECLARE @max_id int; " +
                $"SELECT @max_id = ISNULL(MAX(ID), 0) FROM [dbo].[{tableName}]; " +
                $"DBCC CHECKIDENT ('[dbo].[{tableName}]', RESEED, @max_id);");
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
                await _context.DisposeAsync();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            DisposeAsync(true).GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}
