using Microsoft.EntityFrameworkCore;
using SmartHouse.Core.Model;
using System.Reflection;

namespace SmartHouse.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<SmartDevice> SmartDevices { get; set; }
        public DbSet<SmartProperty> SmartProperties { get; set; }
        public DbSet<TypeOfDevice> TypeOfDevices { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Cycle> Cycles { get; set; }
        public DbSet<Panel> Panels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

        }
    }
}
