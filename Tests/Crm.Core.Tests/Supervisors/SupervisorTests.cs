using Ardalis.GuardClauses;

namespace Crm.Core.Tests.Supervisors
{
    public class SupervisorTests
    {
        private DbContext dbContext => DbContextFactory.GetContext();

        [Fact]
        public void AddNewManagerTest()
        {
            // Arrange
            var supervisor = dbContext.Set<Supervisor>()
                .First();
            var id = Guid.NewGuid();

            // Act
            supervisor.AddNewManager(id);

            // Assert
            Assert.NotNull(supervisor.Managers.FirstOrDefault(manager => manager.Id == id));
            Assert.Throws<InvalidOperationException>(() =>
            {
                supervisor.AddNewManager(id);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                supervisor.AddNewManager(default);
            });
        }

        [Fact]
        public void TransferManagerTest()
        {
            // Arrange
            var supervisors = dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .ToList();
            var fromSupervisor = supervisors[0];
            var toSupervisor = supervisors[1];
            var manager = fromSupervisor.Managers.First();
            var otherManager = toSupervisor.Managers.First();

            // Act
            fromSupervisor.TransferManager(manager.Id, toSupervisor);

            // Assert
            Assert.DoesNotContain(manager, fromSupervisor.Managers);
            Assert.Contains(manager, toSupervisor.Managers);
            Assert.True(manager.SupervisorId == toSupervisor.Id);
            Assert.Throws<NotFoundException>(() =>
            {
                fromSupervisor.TransferManager(otherManager.Id, toSupervisor);
            });
        }

        [Fact]
        public void AssignClientTest()
        {
            // Arrange
            var supervisor = dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .First();
            var manager = supervisor.Managers.First();
            var client = new Client("Test", new ContactInfo("test@mail.com", "+71112223344"));
            var order = client.PlaceOrder("New order");
            var alreadyAssignedClient = manager.Clients.First();

            // Act
            supervisor.AssignClient(manager.Id, client);

            // Assert
            Assert.True(client.ManagerId == manager.Id);
            Assert.Contains(client, manager.Clients);
            Assert.DoesNotContain(order, client.CreatedOrders);
            Assert.True(manager.OrdersInWork.Last().Description == order.Description
                && manager.OrdersInWork.Last().Created == order.Created
                && manager.OrdersInWork.Last().ClientId == order.ClientId);
            Assert.Contains(manager.OrdersInWork.Last(), client.OrdersInWork);
            Assert.Throws<InvalidOperationException>(() =>
            {
                supervisor.AssignClient(manager.Id, alreadyAssignedClient);
            });
        }

        [Fact]
        public void TransferClientTest()
        {
            // Arrange
            var supervisor = dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .ThenInclude(m => m.Clients)
                .ThenInclude(c => c.OrdersInWork)
                .First();
            var fromManager = supervisor.Managers.ElementAt(0);
            var toManager = supervisor.Managers.ElementAt(1);
            var client = fromManager.Clients.First();
            var otherClient = toManager.Clients.First();

            // Act
            supervisor.TransferClient(fromManager.Id, toManager.Id, client.Id);

            // Assert
            Assert.True(client.ManagerId == toManager.Id);
            Assert.DoesNotContain(client, fromManager.Clients);
            Assert.Contains(client, toManager.Clients);
            Assert.True(client.OrdersInWork.All(order => !fromManager.OrdersInWork.Contains(order)));
            Assert.True(client.OrdersInWork.All(order => toManager.OrdersInWork.Contains(order)));
            Assert.True(client.CompletedOrders.All(order => !toManager.CompletedOrders.Contains(order)));
            Assert.True(client.CompletedOrders.All(order => fromManager.CompletedOrders.Contains(order)));
            Assert.Throws<NotFoundException>(() =>
            {
                supervisor.TransferClient(fromManager.Id, toManager.Id, otherClient.Id);
            });
        }
    }
}
