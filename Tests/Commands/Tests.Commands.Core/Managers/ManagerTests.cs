using Ardalis.GuardClauses;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Orders;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Core.Managers
{
    public class ManagerTests
    {
        private DbContext dbContext => DbContextFactory.GetContext();

        [Fact]
        public void CompleteOrderTest()
        {
            // Arrange
            var manager = dbContext.Set<Manager>()
                .Include(manager => manager.OrdersInWork)
                .Include(manager => manager.Clients)
                .First();
            var client = manager.Clients.First();
            var orderInWork = client.OrdersInWork.First();

            // Act
            var completedOrder = manager.CompleteOrder(orderInWork.Id, client.Id, CompletedOrder.CompletionStatus.Fulfilled, "Fulfilled");

            // Assert 
            Assert.DoesNotContain(orderInWork, manager.OrdersInWork);
            Assert.DoesNotContain(orderInWork, client.OrdersInWork);
            Assert.Contains(completedOrder, manager.CompletedOrders);
            Assert.True(completedOrder.ClientId == client.Id
                && completedOrder.Created == orderInWork.Created
                && completedOrder.Assigned == orderInWork.Assigned
                && completedOrder.Description == orderInWork.Description);
            Assert.Contains(completedOrder, client.CompletedOrders);
            Assert.Throws<NotFoundException>(() =>
            {
                manager.CompleteOrder(orderInWork.Id, client.Id, CompletedOrder.CompletionStatus.Fulfilled, "Fulfilled");
            });
        }

        [Fact]
        public void TakeOrderTest()
        {
            // Arrange
            var manager = dbContext.Set<Manager>()
                .Include(manager => manager.OrdersInWork)
                .Include(manager => manager.Clients)
                .ThenInclude(client => client.CreatedOrders)
                .First();
            var client = manager.Clients.First();
            var createdOrder = client.CreatedOrders.First();

            // Act
            var orderInWork = manager.TakeOrder(createdOrder.Id, client.Id);

            // Assert
            Assert.DoesNotContain(createdOrder, client.CreatedOrders);
            Assert.Contains(orderInWork, manager.OrdersInWork);
            Assert.True(orderInWork.Created == createdOrder.Created
                && orderInWork.Description == createdOrder.Description
                && orderInWork.ClientId == client.Id);
            Assert.Contains(orderInWork, client.OrdersInWork);
            Assert.Throws<NotFoundException>(() =>
            {
                manager.TakeOrder(createdOrder.Id, client.Id);
            });
        }

        [Fact]
        public void SetOrderDescriptionTest()
        {
            // Arrange
            var manager = dbContext.Set<Manager>()
                .Include(manager => manager.OrdersInWork)
                .First();
            var orderInWork = manager.OrdersInWork.First();

            // Act
            manager.SetOrderDescription(orderInWork.Id, "New description");

            // Assert
            Assert.True(orderInWork.Description == "New description");
            Assert.Throws<ArgumentException>(() =>
            {
                manager.SetOrderDescription(orderInWork.Id, "");
            });
            Assert.Throws<NotFoundException>(() =>
            {
                manager.SetOrderDescription(Guid.NewGuid(), "New description");
            });
        }

        [Fact]
        public void SetClientNameTest()
        {
            // Arrange
            var manager = dbContext.Set<Manager>()
                .Include(manager => manager.Clients)
                .First();
            var client = manager.Clients.First();

            // Act
            manager.SetClientName(client.Id, "New name");

            // Assert
            Assert.True(client.Name == "New name");
            Assert.Throws<ArgumentException>(() =>
            {
                manager.SetClientName(client.Id, "");
            });
            Assert.Throws<NotFoundException>(() =>
            {
                manager.SetClientName(Guid.NewGuid(), "New name");
            });
        }

        [Fact]
        public void SetClientContactInfoTest()
        {
            // Arrange
            var manager = dbContext.Set<Manager>()
                .Include(manager => manager.Clients)
                .First();
            var client = manager.Clients.First();
            var newContactInfo = new ContactInfo("jake@mail.com", "+71112223344");

            // Act
            manager.SetClientContactInfo(client.Id, newContactInfo.Email, newContactInfo.PhoneNumber);

            // Assert
            Assert.True(client.ContactInfo == newContactInfo);
            Assert.Throws<NotFoundException>(() =>
            {
                manager.SetClientContactInfo(Guid.NewGuid(), newContactInfo.Email, newContactInfo.PhoneNumber);
            });
        }
    }
}
