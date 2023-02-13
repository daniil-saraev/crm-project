using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Core.Supervisors;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Crm.Data.Context
{
    internal class DataContext : DbContext
    {
        internal DataContext (DbContextOptions<DataContext> options) : base(options) { }

        public DataContext() : base() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Crm;Integrated Security=True;");
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<CreatedOrder> CreatedOrders => Set<CreatedOrder>();
        public DbSet<OrderInWork> OrdersInWork => Set<OrderInWork>();
        public DbSet<CompletedOrder> CompletedOrders => Set<CompletedOrder>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Supervisor> Supervisors => Set<Supervisor>();
    }
}
