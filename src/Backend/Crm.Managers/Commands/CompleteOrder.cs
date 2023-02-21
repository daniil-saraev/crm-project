using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Managers.Queries;
using Crm.Shared.Repository;
using MediatR;
using static Crm.Core.Orders.CompletedOrder;

namespace Crm.Managers.Commands
{
    public record CompleteOrderRequest(
    Guid ManagerId,
    Guid ClientId,
    Guid OrderInWorkId,
    CompletionStatus Status,
    string Comment) : IRequest<Result<Guid>>;

    internal class CompleteOrderHandler : IRequestHandler<CompleteOrderRequest, Result<Guid>>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public CompleteOrderHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result<Guid>> Handle(CompleteOrderRequest request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithClientAndOrdersInWork(request.ManagerId, request.ClientId, request.OrderInWorkId, cancellationToken);
            var order = manager.CompleteOrder(request.OrderInWorkId, request.ClientId, request.Status, request.Comment);
            return await SaveChangesAndReturnSuccess(manager, order, cancellationToken);
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

        private async Task<Result<Guid>> SaveChangesAndReturnSuccess(Manager manager, CompletedOrder order, CancellationToken cancellationToken)
        {
            await _writeManager.Update(manager, cancellationToken);
            await _writeManager.SaveChanges(cancellationToken);
            return Result.Success(order.Id);
        }
    }
}
