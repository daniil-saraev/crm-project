﻿using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.ExceptionHandler;
using Crm.Shared.Repository;
using Crm.Supervisors.Commands;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Shared.Context;

namespace Crm.Supervisors.Tests.Commands
{
    public class AddNewManagerHandlerTest
    {
        private readonly Mock<IReadRepository<Supervisor>> _readRepository = new();
        private readonly Mock<IWriteRepository<Supervisor>> _writeRepository = new();
        private readonly ExceptionHandlerBehavior<AddNewManagerRequest> _exceptionHandler;
        private readonly AddNewManagerHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public AddNewManagerHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new AddNewManagerHandler(_writeRepository.Object, _readRepository.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            var supervisor = Setup();
            var managerAccountId = Guid.NewGuid();
            var request = new AddNewManagerRequest(supervisor.Id, managerAccountId);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Supervisor>(s => s.Id == supervisor.Id 
                && s.Managers.Any(m => m.Id == managerAccountId)),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfManagerAlreadyExists()
        {
            // Arrange
            var supervisor = Setup();
            var managerAccountId = supervisor.Managers.First().Id;
            var request = new AddNewManagerRequest(supervisor.Id, managerAccountId);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfSupervisorNotFound()
        {
            // Arrange
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), default))
                .Returns(Task.FromResult<Supervisor?>(null));
            var managerAccountId = Guid.NewGuid();
            var request = new AddNewManagerRequest(Guid.NewGuid(), managerAccountId);

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
            var supervisor = Setup();
            var managerAccountId = Guid.Empty;
            var request = new AddNewManagerRequest(supervisor.Id, managerAccountId);

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
            var supervisor = Setup();
            var managerAccountId = Guid.NewGuid();
            var request = new AddNewManagerRequest(supervisor.Id, managerAccountId);
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
        }

        private Supervisor Setup()
        {
            var supervisor = _dbContext.Set<Supervisor>()
                .Include(s => s.Managers)
                .First();

            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Supervisor>>(), default))
                .ReturnsAsync(supervisor);
            return supervisor;
        }
    }
}