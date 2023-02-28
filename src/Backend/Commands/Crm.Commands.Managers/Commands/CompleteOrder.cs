using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Managers;
using Crm.Messages.Managers;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MediatR;
using static Crm.Commands.Core.Orders.CompletedOrder;

namespace Crm.Commands.Managers.Commands
{
    public record CompleteOrderCommand(
    Guid ManagerId,
    Guid ClientId,
    Guid OrderInWorkId,
    CompletionStatus Status,
    string Comment) : IRequest<Result<Guid>>;

    public record ManagerWithClientAndOrdersInWorkQuery(
    Guid ManagerId,
    Guid ClientId,
    Guid OrderInWorkId) : ISingleQuery<Manager>;

    internal class CompleteOrderHandler : IRequestHandler<CompleteOrderCommand, Result<Guid>>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;
        private readonly IMessageBus _eventBus;

        public CompleteOrderHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager, IMessageBus eventBus)
        {
            _readManager = readManager;
            _writeManager = writeManager;
            _eventBus = eventBus;
        }

        public async Task<Result<Guid>> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithClientAndOrdersInWork(request.ManagerId, request.ClientId, request.OrderInWorkId, cancellationToken);
            var order = manager.CompleteOrder(request.OrderInWorkId, request.ClientId, request.Status, request.Comment);
            await _writeManager.Update(manager, cancellationToken);
            await _writeManager.SaveChanges(cancellationToken);
            await _eventBus.Publish(new OrderCompletedEvent(manager.Id, order.Id), cancellationToken);
            return Result.Success(order.Id);
        }

        private async Task<Manager> GetManagerWithClientAndOrdersInWork(Guid managerId, Guid clientId, Guid orderInWorkId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
            new ManagerWithClientAndOrdersInWorkQuery(managerId, clientId, orderInWorkId),
                    cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }
    }
}
