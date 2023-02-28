using Crm.Commands.Core.Supervisors;
using Crm.Messages.Clients;
using Crm.Shared.Messages;
using Crm.Shared.Models;
using Crm.Shared.Repository;
using MassTransit;

namespace Tests.Commands.Clients.Commands
{
    public class CreateOrderHandlerTest
    {
        private readonly Mock<IReadRepository<Client>> _readRepository = new();
        private readonly Mock<IWriteRepository<Client>> _writeRepository = new();
        private readonly Mock<IMessageBus> _eventBus = new();
        private readonly Mock<ConsumeContext<CreateOrderCommand>> _context = new();
        private readonly CreateOrderHandler _commandHandler;

        public CreateOrderHandlerTest()
        {
            _commandHandler = new CreateOrderHandler(_readRepository.Object, _writeRepository.Object, _eventBus.Object);
        }

        [Fact]
        public async Task HandleIfClientHasManagerTest()
        {
            // Arrange
            var client = CreateClientWithManager();
            var request = new CreateOrderCommand(client.Name, client.ContactInfo.Email, client.ContactInfo.PhoneNumber, "New order");
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .ReturnsAsync(client);
            _context.Setup(ctx => ctx.Message).Returns(request);

            // Act
            var result = _commandHandler.Consume(_context.Object);
            await result;

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
            _writeRepository.Verify(r => r.Update(It.Is<Client>(c => c.Id == client.Id && c.CreatedOrders.Single() != null),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<ExistingClientPlacedOrderEvent>(@event =>
                @event.CreatedOrderId == client.CreatedOrders.Single().Id && @event.ClientId == client.Id),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfClientHasNoManagerTest()
        {
            // Arrange
            var request = new CreateOrderCommand("Name", "email@mail.com", "+79998887711", "New order");
            var client = new Client("Name", new ContactInfo("email@mail.com", "+79998887711"));
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .ReturnsAsync(client);
            _context.Setup(ctx => ctx.Message).Returns(request);

            // Act
            var result = _commandHandler.Consume(_context.Object);
            await result;

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
            _writeRepository.Verify(r => r.Update(It.Is<Client>(c => c.Id == client.Id && c.CreatedOrders.Single() != null),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<NewClientPlacedOrderEvent>(@event
                => @event.CreatedOrderId != Guid.Empty && @event.ClientId != Guid.Empty),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfClientDoesNotExistTest()
        {
            // Arrange
            var request = new CreateOrderCommand("Name", "email@mail.com", "+79998887711", "New order");
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .Returns(Task.FromResult<Client?>(null));
            _context.Setup(ctx => ctx.Message).Returns(request);

            // Act
            var result = _commandHandler.Consume(_context.Object);
            await result;

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
            _writeRepository.Verify(r => r.Update(It.Is<Client>(c => c.CreatedOrders.Single() != null),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _eventBus.Verify(e => e.Publish(It.Is<NewClientPlacedOrderEvent>(@event 
                => @event.CreatedOrderId != Guid.Empty && @event.ClientId != Guid.Empty),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfRequestIsInvalid()
        {
            // Arrange
            var request = new CreateOrderCommand("", "", "", "");
            _context.Setup(ctx => ctx.Message).Returns(request);

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await _commandHandler.Consume(_context.Object));
            _writeRepository.VerifyNoOtherCalls();
            _eventBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            var request = new CreateOrderCommand("Name", "email@mail.com", "+79998887711", "New order");
            var client = new Client("Name", new ContactInfo("email@mail.com", "+79998887711"));
            _readRepository.Setup(x => x.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .ReturnsAsync(client);
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            _context.Setup(ctx => ctx.Message).Returns(request);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await _commandHandler.Consume(_context.Object));
            _eventBus.VerifyNoOtherCalls();
        }

        private static Client CreateClientWithManager()
        {
            var client = new Client("Name", new ContactInfo("email@mail.com", "+79998887711"));
            var managerId = Guid.NewGuid();
            var supervisor = Supervisor.New(Guid.NewGuid());
            supervisor.AddNewManager(managerId);
            supervisor.AssignClient(managerId, client);
            return client;
        }
    }
}
