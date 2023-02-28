using Ardalis.GuardClauses;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Orders;
using Crm.Commands.Managers.EventHandlers;
using Crm.Messages.Clients;
using Crm.Messages.Managers;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Managers.EventHandlers
{
    public class ExistingClientPlacedOrderHandlerTest
    {
        private readonly Mock<IWriteRepository<Manager>> _writeManager = new();
        private readonly Mock<IReadRepository<Manager>> _readManager = new();
        private readonly Mock<IMessageBus> _eventBus = new();
        private readonly Mock<ConsumeContext<ExistingClientPlacedOrderEvent>> _context = new();
        private readonly ExistingClientPlacedOrderHandler _handler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public ExistingClientPlacedOrderHandlerTest()
        {
            _handler = new ExistingClientPlacedOrderHandler(_writeManager.Object, _readManager.Object, _eventBus.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new ExistingClientPlacedOrderEvent(client.Id, order.Id, manager.Id);
            _context.Setup(ctx => ctx.Message).Returns(notification);

            // Act
            var task = _handler.Consume(_context.Object);
            await task;

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            _writeManager.Verify(r => r.Update(It.Is<Manager>(m => m.OrdersInWork.Last().Created == order.Created), default));
            _writeManager.Verify(r => r.SaveChanges(default));
            _eventBus.Verify(e => e.Publish(It.Is<NewOrderAssignedEvent>(o => 
                o.ManagerId == manager.Id && o.OrderId == manager.OrdersInWork.Last().Id), 
                default));
        }

        [Fact]
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            _readManager.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Manager?>(null));
            var notification = new ExistingClientPlacedOrderEvent(client.Id, order.Id, manager.Id);
            _context.Setup(ctx => ctx.Message).Returns(notification);

            // Assert
            await Assert.ThrowsAnyAsync<NotFoundException>(async () => await _handler.Consume(_context.Object));
            _writeManager.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfClientNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new ExistingClientPlacedOrderEvent(Guid.NewGuid(), order.Id, manager.Id);
            _context.Setup(ctx => ctx.Message).Returns(notification);

            // Assert
            await Assert.ThrowsAnyAsync<NotFoundException>(async () => await _handler.Consume(_context.Object));
            _writeManager.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfOrderNotFound()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            var notification = new ExistingClientPlacedOrderEvent(client.Id, Guid.NewGuid(), manager.Id);
            _context.Setup(ctx => ctx.Message).Returns(notification);

            // Assert
            await Assert.ThrowsAnyAsync<NotFoundException>(async () => await _handler.Consume(_context.Object));
            _writeManager.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Manager manager, Client client, CreatedOrder order) = Setup();
            _writeManager.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var notification = new ExistingClientPlacedOrderEvent(client.Id, order.Id, manager.Id);
            _context.Setup(ctx => ctx.Message).Returns(notification);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await _handler.Consume(_context.Object));
            _eventBus.VerifyNoOtherCalls();
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
