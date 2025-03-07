using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.db
{
    public class HaosDbContext : DbContext
    {
        public HaosDbContext(DbContextOptions<HaosDbContext> options) : base(options) { }

        public DbSet<DealerModel> Dealers { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserEmployee> UserEmployees { get; set; }
        public DbSet<CarModel> Cars { get; set; }
        public DbSet<PartDamage> PartDamages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PartDamage>(entity =>
            {
                entity.Property(e => e.PartTypeId)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.DamageTypeId)
                    .HasColumnType("int")
                    .IsRequired();
                entity.HasOne<CarModel>()  // Car navigasyon property'si olmadan ilişki tanımlama
                    .WithMany(c => c.PartDamages)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Username)
            .IsUnique();
        }
    }
}