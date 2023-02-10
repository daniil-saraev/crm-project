using Crm.Core.Models.Clients;
using Crm.Core.Models.Managers;
using Crm.Core.Models.Orders;
using Crm.Core.Models.Supervisors;
using Microsoft.EntityFrameworkCore;

namespace Crm.Data.Context
{
    public class DataContext : DbContext
    {
        internal DataContext (DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<CreatedOrder> CreatedOrders => Set<CreatedOrder>();
        public DbSet<OrderInWork> OrdersInWork => Set<OrderInWork>();
        public DbSet<CompletedOrder> CompletedOrders => Set<CompletedOrder>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Supervisor> Supervisors => Set<Supervisor>();
    }
}
