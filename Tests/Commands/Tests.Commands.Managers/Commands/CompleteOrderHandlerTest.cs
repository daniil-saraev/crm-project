using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.ExceptionHandler;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Orders;
using Crm.Commands.Managers.Commands;
using Crm.Messages.Managers;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;
using static Crm.Commands.Core.Orders.CompletedOrder;

namespace Tests.Commands.Managers.Commands
{
    public class CompleteOrderHandlerTest
    {
        private readonly Mock<IReadRepository<Manager>> _readRepository = new();
        private readonly Mock<IWriteRepository<Manager>> _writeRepository = new();
        private readonly Mock<IMessageBus> _eventBus = new();
        private readonly ExceptionHandlerBehaviorReturnResultWithGuid<CompleteOrderCommand, Result<Guid>> _exceptionHandler;
        private readonly CompleteOrderHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public CompleteOrderHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new CompleteOrderHandler(_readRepository.Object, _writeRepository.Object, _eventBus.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            var request = new CompleteOrderCommand(
                manager.Id.ToString(), client.Id.ToString(), order.Id.ToString(), CompletionStatus.Fulfilled.ToString(), "Test");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Ok);
            Assert.True(result.Value != Guid.Empty);
            _writeRepository.Verify(r => r.Update(It.Is<Manager>(m => m.Id == manager.Id && m.CompletedOrders.Last().Id == result.Value),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<OrderCompletedEvent>(o => o.CompletedOrderId == result.Value && o.ManagerId == manager.Id), default));
        }

        [Fact]
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .Returns(Task.FromResult<Manager?>(null));
            var request = new CompleteOrderCommand(
                Guid.NewGuid().ToString(), client.Id.ToString(), order.Id.ToString(), CompletionStatus.Fulfilled.ToString(), "Test");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfClientNotFound()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            var request = new CompleteOrderCommand(
                manager.Id.ToString(), Guid.NewGuid().ToString(), order.Id.ToString(), CompletionStatus.Fulfilled.ToString(), "Test");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfOrderNotFound()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            var request = new CompleteOrderCommand(
                manager.Id.ToString(), client.Id.ToString(), Guid.NewGuid().ToString(), CompletionStatus.Fulfilled.ToString(), "Test");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfInvalidRequest()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            var request = new CompleteOrderCommand(
                manager.Id.ToString(), client.Id.ToString(), order.Id.ToString(), CompletionStatus.Fulfilled.ToString(), "");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Invalid);
            Assert.NotEmpty(result.ValidationErrors);
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Manager manager, Client client, OrderInWork order) = Setup();
            var request = new CompleteOrderCommand(
                manager.Id.ToString(), client.Id.ToString(), order.Id.ToString(), CompletionStatus.Fulfilled.ToString(), "Test");
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
            _eventBus.VerifyNoOtherCalls();
        }

        private (Manager mananager, Client client, OrderInWork orderInWork) Setup()
        {
            var manager = _dbContext.Set<Manager>()
                        .Include(manager => manager.OrdersInWork)
                        .Include(manager => manager.Clients)
                        .ThenInclude(client => client.OrdersInWork)
                        .First();
            var client = manager.Clients.First();
            var order = client.OrdersInWork.First();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .ReturnsAsync(manager);
            return (manager, client, order);
        }
    }
}
