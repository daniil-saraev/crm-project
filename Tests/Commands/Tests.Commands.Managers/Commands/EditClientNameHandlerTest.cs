using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.ExceptionHandler;
using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Commands.Shared.Context;

namespace Tests.Commands.Managers.Commands
{
    public class EditClientNameHandlerTest
    {
        private readonly Mock<IReadRepository<Manager>> _readRepository = new();
        private readonly Mock<IWriteRepository<Manager>> _writeRepository = new();
        private readonly ExceptionHandlerBehavior<EditClientNameCommand> _exceptionHandler;
        private readonly EditClientNameHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public EditClientNameHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new EditClientNameHandler(_readRepository.Object, _writeRepository.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Manager manager, Client client) = Setup();
            const string name = "New name";
            var request = new EditClientNameCommand(manager.Id, client.Id, name);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Manager>(m => m.Id == manager.Id
                && m.Clients.First(c => c.Id == client.Id).Name == name),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Manager manager, Client client) = Setup();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .Returns(Task.FromResult<Manager?>(null));
            var request = new EditClientNameCommand(Guid.NewGuid(), client.Id, "New name");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfClientNotFound()
        {
            // Arrange
            (Manager manager, Client client) = Setup();
            var request = new EditClientNameCommand(manager.Id, Guid.NewGuid(), "New name");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfInvalidRequest()
        {
            // Arrange
            (Manager manager, Client client) = Setup();
            var request = new EditClientNameCommand(manager.Id, client.Id, "");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Invalid);
            Assert.NotEmpty(result.ValidationErrors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Manager manager, Client client) = Setup();
            var request = new EditClientNameCommand(manager.Id, client.Id, "New name");
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
        }

        private (Manager mananager, Client client) Setup()
        {
            var manager = _dbContext.Set<Manager>()
                        .Include(manager => manager.Clients)
                        .First();
            var client = manager.Clients.First();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .ReturnsAsync(manager);
            return (manager, client);
        }
    }
}
