using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.ExceptionHandler;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Models;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Supervisors.Commands
{
    public class AssignClientHandlerTest
    {
        private readonly Mock<IReadRepository<Supervisor>> _readRepository = new();
        private readonly Mock<IWriteRepository<Supervisor>> _writeRepository = new();
        private readonly Mock<IReadRepository<Client>> _clientRepository = new();
        private readonly Mock<IMessageBus> _messageBus = new();
        private readonly ExceptionHandlerBehaviorReturnResult<AssignClientCommand, Result> _exceptionHandler;
        private readonly AssignClientHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public AssignClientHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new AssignClientHandler(
                _writeRepository.Object, _readRepository.Object, _clientRepository.Object, _messageBus.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Supervisor supervisor, Manager manager, Client client) = Setup();
            var request = new AssignClientCommand(supervisor.Id.ToString(), manager.Id.ToString(), client.Id.ToString());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Supervisor>(s => 
                s.Managers.First(m => m.Id == manager.Id).Clients.Any(c => c.Id == client.Id)),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
            _messageBus.Verify(m => m.Publish(It.Is<NewClientAssignedEvent>(e =>
                e.ManagerId == manager.Id && e.ClientId == client.Id),
                default));
        }

        [Fact]
        public async Task HandleIfSupervisorNotFound()
        {
            // Arrange
            (Supervisor supervisor, Manager manager, Client client) = Setup();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), default))
                .Returns(Task.FromResult<Supervisor?>(null));
            var request = new AssignClientCommand(supervisor.Id.ToString(), manager.Id.ToString(), client.Id.ToString());

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
        public async Task HandleIfClientNotFound()
        {
            // Arrange
            (Supervisor supervisor, Manager manager, Client client) = Setup();
            _clientRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Client>>(), default))
                .Returns(Task.FromResult<Client?>(null));
            var request = new AssignClientCommand(supervisor.Id.ToString(), manager.Id.ToString(), client.Id.ToString());

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
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Supervisor supervisor, Manager manager, Client client) = Setup();
            var request = new AssignClientCommand(supervisor.Id.ToString(), Guid.NewGuid().ToString(), client.Id.ToString());

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
            (Supervisor supervisor, Manager manager, Client client) = Setup();
            var request = new AssignClientCommand(supervisor.Id.ToString(), manager.Id.ToString(), client.Id.ToString());
            _writeRepository.Setup(r => r.SaveChanges(default)).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
            _messageBus.VerifyNoOtherCalls();
        }

        private (Supervisor, Manager, Client) Setup()
        {
            var context = _dbContext;
            var supervisor = context.Set<Supervisor>()
                .Include(s => s.Managers)
                .First();
            var manager = supervisor.Managers.First();
            var client = new Client("Name", new ContactInfo("test@mail.com", "+19998887766"));

            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(supervisor);
            _clientRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Client>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);

            return (supervisor, manager, client);
        }
    }
}
