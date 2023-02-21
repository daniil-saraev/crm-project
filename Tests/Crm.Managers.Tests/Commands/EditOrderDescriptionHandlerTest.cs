using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Managers.Commands;
using Crm.Shared.ExceptionHandler;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tests.Shared.Context;

namespace Crm.Managers.Tests.Commands
{
    public class EditOrderDescriptionHandlerTest
    {
        private readonly Mock<IReadRepository<Manager>> _readRepository = new();
        private readonly Mock<IWriteRepository<Manager>> _writeRepository = new();
        private readonly ExceptionHandlerBehavior<EditOrderDescriptionRequest> _exceptionHandler;
        private readonly EditOrderDescriptionHandler _commandHandler;
        private DbContext _dbContext => DbContextFactory.GetContext();

        public EditOrderDescriptionHandlerTest()
        {
            _exceptionHandler = new();
            _commandHandler = new EditOrderDescriptionHandler(_readRepository.Object, _writeRepository.Object);
        }

        [Fact]
        public async Task HandleSuccessfulTest()
        {
            // Arrange
            (Manager manager, OrderInWork order) = Setup();
            const string description = "Test";
            var request = new EditOrderDescriptionRequest(manager.Id, order.Id, description);

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Ok);
            _writeRepository.Verify(r => r.Update(It.Is<Manager>(m => m.Id == manager.Id 
                && m.OrdersInWork.First(o => o.Id == order.Id).Description == description),
                It.IsAny<CancellationToken>()));
            _writeRepository.Verify(r => r.SaveChanges(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HandleIfManagerNotFound()
        {
            // Arrange
            (Manager manager, OrderInWork order) = Setup();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .Returns(Task.FromResult<Manager?>(null));
            var request = new EditOrderDescriptionRequest(Guid.NewGuid(), order.Id, "Test");

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.NotFound);
            Assert.NotEmpty(result.Errors);
            _writeRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task HandleIfOrderNotFound()
        {
            // Arrange
            (Manager manager, OrderInWork order) = Setup();
            var request = new EditOrderDescriptionRequest(manager.Id, Guid.NewGuid(), "Test");

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
            (Manager manager, OrderInWork order) = Setup();
            var request = new EditOrderDescriptionRequest(manager.Id, order.Id, "");

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
            (Manager manager, OrderInWork order) = Setup();
            var request = new EditOrderDescriptionRequest(manager.Id, order.Id, "Test");
            _writeRepository.Setup(r => r.SaveChanges(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            var result = await _exceptionHandler.Handle(request, () => _commandHandler.Handle(request, default), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.Status == ResultStatus.Error);
            Assert.NotEmpty(result.Errors);
        }

        private (Manager mananager, OrderInWork orderInWork) Setup()
        {
            var manager = _dbContext.Set<Manager>()
                        .Include(manager => manager.OrdersInWork)
                        .First();
            var client = manager.Clients.First();
            var order = client.OrdersInWork.First();
            _readRepository.Setup(r => r.Execute(It.IsAny<ISingleQuery<Manager>>(), default))
                .ReturnsAsync(manager);
            return (manager, order);
        }
    }
}
