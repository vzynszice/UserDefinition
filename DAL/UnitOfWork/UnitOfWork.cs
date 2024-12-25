using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.interfaces;
using DAL.Models;
using DAL.Repositories;
using DAL.db;
using Microsoft.EntityFrameworkCore;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HaosDbContext _context;
        private bool _disposed = false;

        // Repository'ler için field'lar
        private IGenericRepository<User> _users;
        private IGenericRepository<Dealer> _dealers;
        private IGenericRepository<Service> _services;
        private IGenericRepository<Employee> _employees;

        public UnitOfWork(HaosDbContext context)
        {
            _context = context;
        }

        // Lazy loading ile repository'leri oluşturma
        public IGenericRepository<User> Users =>
            _users ??= new GenericRepository<User>(_context);

        public IGenericRepository<Dealer> Dealers =>
            _dealers ??= new GenericRepository<Dealer>(_context);

        public IGenericRepository<Service> Services =>
            _services ??= new GenericRepository<Service>(_context);

        public IGenericRepository<Employee> Employees =>
            _employees ??= new GenericRepository<Employee>(_context);

        public async Task ResetIdentityAsync(string tableName)
        {
            // Raw SQL kullanımı
            await _context.Database.ExecuteSqlRawAsync(
                $"DECLARE @max_id int; " +
                $"SELECT @max_id = ISNULL(MAX(ID), 0) FROM [dbo].[{tableName}]; " +
                $"DBCC CHECKIDENT ('[dbo].[{tableName}]', RESEED, @max_id);");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
