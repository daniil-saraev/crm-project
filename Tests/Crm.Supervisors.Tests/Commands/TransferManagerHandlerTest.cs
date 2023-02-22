using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Supervisors;
using Crm.Shared.ExceptionHandler;
using Crm.Shared.Repository;
using Crm.Supervisors.Commands;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Shared.Context;

namespace Crm.Supervisors.Tests.Commands
{
    public class TransferManagerHandlerTest
    {
        private readonly Mock<IReadRepository<Supervisor>> _readRepository = new();
        private readonly Mock<IWriteRepository<Supervisor>> _writeRepository = new();
        private readonly ExceptionHandlerBehavior<TransferManagerRequest> _exceptionHandler;
        private readonly TransferManagerHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public TransferManagerHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new TransferManagerHandler(_writeRepository.Object, _readRepository.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Supervisor fromSupervisor, Supervisor toSupervisor, Manager manager)
                = Setup();
            var request = new TransferManagerRequest(fromSupervisor.Id, toSupervisor.Id, manager.Id);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Supervisor>(s =>
                s.Id == fromSupervisor.Id
                && !s.Managers.Contains(manager)), default));
            _writeRepository.Verify(r => r.Update(It.Is<Supervisor>(s =>
                s.Id == toSupervisor.Id
                && s.Managers.Contains(manager)), default));
            _writeRepository.Verify(r => r.SaveChanges(default));
        }

        [Fact]
        public async Task HandleIfManagerNotFoundTest()
        {
            // Arrange
            (Supervisor fromSupervisor, Supervisor toSupervisor, Manager manager)
                = Setup();
            var request = new TransferManagerRequest(fromSupervisor.Id, toSupervisor.Id, Guid.NewGuid());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfSupervisorNotFoundTest()
        {
            // Arrange
            (Supervisor fromSupervisor, Supervisor toSupervisor, Manager manager)
                = Setup();
            var request = new TransferManagerRequest(fromSupervisor.Id, toSupervisor.Id, manager.Id);
            _readRepository.Setup(r => r.Execute(It.IsAny<ICollectionQuery<Supervisor>>(), default))
                .Returns(Task.FromResult<IEnumerable<Supervisor>>(new List<Supervisor>()));

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfRepositoryThrowsException()
        {
            // Arrange
            (Supervisor fromSupervisor, Supervisor toSupervisor, Manager manager)
                = Setup();
            var request = new TransferManagerRequest(fromSupervisor.Id, toSupervisor.Id, manager.Id);
            _writeRepository.Setup(r => r.SaveChanges(default)).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == Ardalis.Result.ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
        }

        private (Supervisor, Supervisor, Manager) Setup()
        {
            var supervisors = _dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .ToList();
            var fromSupervisor = supervisors[0];
            var toSupervisor = supervisors[1];
            var manager = fromSupervisor.Managers.First();

            _readRepository.Setup(r => r.Execute(It.IsAny<ICollectionQuery<Supervisor>>(), default))
                .ReturnsAsync(new[] {fromSupervisor, toSupervisor});

            return (fromSupervisor, toSupervisor, manager);
        }
    }
}
