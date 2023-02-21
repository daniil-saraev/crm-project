using Ardalis.Result;
using Crm.Core.Clients.Events;
using Crm.Shared.Events;
using Crm.Shared.ExceptionHandler;
using Crm.Shared.Models;
using Crm.Shared.Repository;

namespace Crm.Clients.Tests.Commands
{
    public class CreateOrderHandlerTest
    {
        private readonly Mock<IReadRepository<Client>> _readRepository = new();
        private readonly Mock<IWriteRepository<Client>> _writeRepository = new();
        private readonly Mock<IEventBus> _eventBus = new();
        private readonly ExceptionHandlerBehavior<CreateOrderRequest, Guid> _exceptionHandler;
        private readonly CreateOrderHandler _commandHandler;

        public CreateOrderHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new CreateOrderHandler(_readRepository.Object, _writeRepository.Object, _eventBus.Object);
        }

        [Fact]
        public async Task HandleIfClientExistsTest()
        {
            // Arrange
            var request = new CreateOrderRequest("Name", "email@mail.com", "+79998887711", "New order");
            var client = new Client("Name", new ContactInfo("email@mail.com", "+79998887711"));
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .ReturnsAsync(client);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Ok);
            Assert.True(result.Value != Guid.Empty);
            _writeRepository.Verify(r => r.Update(It.Is<Client>(c => c.Id == client.Id && c.CreatedOrders.Single().Id == result.Value), 
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<OrderCreatedEvent>(@event => @event.OrderId == result.Value && @event.ClientId == client.Id),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfClientDoesNotExistTest()
        {
            // Arrange
            var request = new CreateOrderRequest("Name", "email@mail.com", "+79998887711", "New order");
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .Returns(Task.FromResult<Client?>(null));

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Ok);
            Assert.True(result.Value != Guid.Empty);
            _writeRepository.Verify(r => r.Update(It.Is<Client>(c => c.CreatedOrders.Single().Id == result.Value),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<OrderCreatedEvent>(@event => @event.OrderId == result.Value && @event.ClientId != Guid.Empty),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfRequestIsInvalid()
        {
            // Arrange
            var request = new CreateOrderRequest("", "", "", "");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Invalid);
            Assert.NotEmpty(result.ValidationErrors);
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            var request = new CreateOrderRequest("Name", "email@mail.com", "+79998887711", "New order");
            var client = new Client("Name", new ContactInfo("email@mail.com", "+79998887711"));
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .ReturnsAsync(client);
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
            _eventBus.VerifyNoOtherCalls();
        }
    }
}
