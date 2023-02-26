using Crm.Data.Context;
using Microsoft.EntityFrameworkCore;
using Tests.Shared.Context;

namespace Tests.Commands.Shared.Context
{
    public static class DbContextFactory
    {
        public static DbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedData(context);
            return context;
        }

        private static void SeedData(DataContext context)
        {
            var sample = new SampleData();
            sample.John.AssignManager(sample.JohnManager.Id);
            sample.Ben.AssignManager(sample.BenManager.Id);
            sample.Bill.AssignManager(sample.BillManager.Id);
            sample.Jake.AssignManager(sample.JakeManager.Id);
            context.Clients.AddRange(sample.John, sample.Ben, sample.Bill, sample.Jake);
            context.Supervisors.AddRange(sample.JohnAndBenManagerSupervisor, sample.BillAndJakeManagerSupervisor);
            context.Managers.AddRange(sample.JohnManager, sample.BenManager, sample.BillManager, sample.JakeManager);
            context.CreatedOrders.AddRange(sample.JohnCreatedOrder, sample.BenCreatedOrder, sample.BillCreatedOrder, sample.JakeCreatedOrder);
            context.OrdersInWork.AddRange(sample.JohnOrderInWork, sample.BenOrderInWork, sample.BillOrderInWork, sample.JakeOrderInWork);
            context.CompletedOrders.AddRange(sample.JohnCompletedOrder, sample.BenCompletedOrder, sample.BillCompletedOrder, sample.JakeCompletedOrder);
            context.SaveChanges();
        }
    }
}
