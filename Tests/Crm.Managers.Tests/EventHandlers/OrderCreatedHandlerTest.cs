using Crm.Core.Clients;
using Crm.Core.Clients.Events;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Managers.EventHandlers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Managers.EventHandlers
{
    public class OrderCreatedHandlerTest
    {
        private readonly Mock<IWriteRepository<Manager>> _writeManager = new();
        private readonly Mock<IReadRepository<Manager>> _readManager = new();
        private readonly OrderCreatedHandler _handler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public OrderCreatedHandlerTest()
        {
            _handler = new OrderCreatedHandler(_writeManager.Object, _readManager.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new OrderCreatedEvent(client.Id, order.Id, manager.Id);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _writeManager.Verify(r => r.Update(It.Is<Manager>(m => m.OrdersInWork.Last().Created == order.Created), default));
            _writeManager.Verify(r => r.SaveChanges(default));
        }

        [Fact]
        public async Task HandleIfManagerIdIsNull()
        {
            // Arrange
            var notification = new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), null);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _readManager.VerifyNoOtherCalls();
            _writeManager.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            _readManager.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Manager?>(null));
            var notification = new OrderCreatedEvent(client.Id, order.Id, manager.Id);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _writeManager.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfClientNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new OrderCreatedEvent(Guid.NewGuid(), order.Id, manager.Id);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _writeManager.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfOrderNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new OrderCreatedEvent(client.Id, Guid.NewGuid(), manager.Id);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _writeManager.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            _writeManager.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var notification = new OrderCreatedEvent(client.Id, order.Id, manager.Id);
            var task = _handler.Handle(notification, default);

            // Act
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        private (Manager, Client, CreatedOrder) Setup()
        {
            var manager = _dbContext.Set<Manager>()
                .Include(m => m.Clients)
                .First();
            var client = manager.Clients.First();
            var order = client.PlaceOrder("New order");

            _readManager.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(manager);

            return (manager, client, order);
        }
    }
}
