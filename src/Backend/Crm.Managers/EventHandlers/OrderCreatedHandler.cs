using Ardalis.GuardClauses;
using Crm.Core.Clients.Events;
using Crm.Core.Managers;
using Crm.Shared.Events;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.EventHandlers
{
    internal class OrderCreatedHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IWriteRepository<Manager> _writeManager;
        private readonly IReadRepository<Manager> _readManager;

        public OrderCreatedHandler(
            IWriteRepository<Manager> writeManager,
            IReadRepository<Manager> readManager)
        {
            _writeManager = writeManager;
            _readManager = readManager;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                if (!notification.ManagerId.HasValue)
                    return;
                var manager = await GetManager(notification.ManagerId.Value, notification.ClientId, cancellationToken);
                manager.TakeOrder(notification.OrderId, notification.ClientId);
                await _writeManager.Update(manager, cancellationToken);
                await _writeManager.SaveChanges(cancellationToken);
            }
            catch (Exception)
            {
                // todo: logging
                return;
            }
        }

        private async Task<Manager> GetManager(Guid managerId, Guid clientId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
                new ManagerWithClientWithCreatedOrdersQuery(managerId, clientId),
                cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }
    }

    file record ManagerWithClientWithCreatedOrdersQuery(
        Guid ManagerId,
        Guid ClientId) : ISingleQuery<Manager>;

    file class ManagerWithClientWithCreatedOrdersHandler : ISingleQueryHandler<ManagerWithClientWithCreatedOrdersQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerWithClientWithCreatedOrdersHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerWithClientWithCreatedOrdersQuery request, CancellationToken cancellationToken)
        {
            var manager = await _context.Set<Manager>()
                    .Where(manager => manager.Id == request.ManagerId)
                    .IncludeFilter(manager => manager.Clients
                        .Where(client => client.Id == request.ClientId))
                    .IncludeFilter(manager => manager.Clients
                        .Where(client => client.Id == request.ClientId)
                            .Select(client => client.CreatedOrders))
                    .SingleOrDefaultAsync(cancellationToken);
            return manager;
        }
    }
}
