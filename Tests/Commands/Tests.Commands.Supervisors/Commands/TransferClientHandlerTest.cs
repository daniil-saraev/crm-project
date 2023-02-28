using Crm.Commands.Core.Clients;
using Crm.Commands.Core.ExceptionHandler;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Supervisors.Commands
{
    public class TransferClientHandlerTest
    {
        private readonly Mock<IReadRepository<Supervisor>> _readRepository = new();
        private readonly Mock<IWriteRepository<Supervisor>> _writeRepository = new();
        private readonly Mock<IMessageBus> _messageBus = new();
        private readonly ExceptionHandlerBehavior<TransferClientCommand> _exceptionHandler;
        private readonly TransferClientHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public TransferClientHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new TransferClientHandler(_writeRepository.Object, _readRepository.Object, _messageBus.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Supervisor supervisor, Manager fromManager, Manager toManager, Client client)
                = Setup();
            var request = new TransferClientCommand(supervisor.Id, fromManager.Id, toManager.Id, client.Id);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Supervisor>(s =>
                !s.Managers.First(m => m.Id == fromManager.Id).Clients.Contains(client)
                && s.Managers.First(m => m.Id == toManager.Id).Clients.Contains(client)), default));
            _writeRepository.Verify(r => r.SaveChanges(default));
            _messageBus.Verify(m => m.Publish(It.Is<NewClientAssignedEvent>(e =>
                e.ManagerId == toManager.Id && e.ClientId == client.Id),
                default));
        }

        [Fact]
        public async Task HandleIfManagerNotFoundTest()
        {
            // Arrange
            (Supervisor supervisor, Manager fromManager, Manager toManager, Client client)
                = Setup();
            var request = new TransferClientCommand(supervisor.Id, Guid.NewGuid(), toManager.Id, client.Id);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _messageBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfClientNotFoundTest()
        {
            // Arrange
            (Supervisor supervisor, Manager fromManager, Manager toManager, Client client)
                = Setup();
            var request = new TransferClientCommand(supervisor.Id, fromManager.Id, toManager.Id, Guid.NewGuid());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _messageBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfSupervisorNotFoundTest()
        {
            // Arrange
            (Supervisor supervisor, Manager fromManager, Manager toManager, Client client)
                = Setup();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), default))
                .Returns(Task.FromResult<Supervisor?>(null));
            var request = new TransferClientCommand(supervisor.Id, fromManager.Id, toManager.Id, client.Id);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
            _messageBus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Supervisor supervisor, Manager fromManager, Manager toManager, Client client)
                = Setup();
            var request = new TransferClientCommand(supervisor.Id, fromManager.Id, toManager.Id, client.Id);
            _writeRepository.Setup(r => r.SaveChanges(default)).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
            _messageBus.VerifyNoOtherCalls();
        }

        private (Supervisor, Manager, Manager, Client) Setup()
        {
            var supervisor = _dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .ThenInclude(m => m.Clients)
                .First();
            var fromManager = supervisor.Managers.First();
            var toManager = supervisor.Managers.Last();
            var client = fromManager.Clients.First();

            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), default))
                .ReturnsAsync(supervisor);

            return (supervisor, fromManager, toManager, client);
        }
    }
}
