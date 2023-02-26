using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Orders;
using Crm.Commands.Core.Supervisors;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Crm.Commands.Data.Context
{
    internal class DataContext : DbContext
    {
        internal DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<CreatedOrder> CreatedOrders => Set<CreatedOrder>();
        public DbSet<OrderInWork> OrdersInWork => Set<OrderInWork>();
        public DbSet<CompletedOrder> CompletedOrders => Set<CompletedOrder>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Supervisor> Supervisors => Set<Supervisor>();
    }
}
