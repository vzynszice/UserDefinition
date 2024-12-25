using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.db
{
    public class HaosDbContext : DbContext
    {
        public HaosDbContext(DbContextOptions<HaosDbContext> options) : base(options) { }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserEmployee> UserEmployees { get; set; }
    }
}
