using Ardalis.GuardClauses;
using Crm.Core.Clients;
using Crm.Core.Clients.Events;
using Crm.Core.Managers;
using Crm.Managers.Queries;
using Crm.Shared.Events;
using Crm.Shared.Repository;

namespace Crm.Managers.EventHandlers
{
    internal class OrderCreatedHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IWriteRepository<Manager> _writeManager;
        private readonly IReadRepository<Manager> _readManager;
        private readonly IReadRepository<Client> _readClient;

        public OrderCreatedHandler(
            IWriteRepository<Manager> writeManager,
            IReadRepository<Manager> readManager,
            IReadRepository<Client> readClient)
        {
            _writeManager = writeManager;
            _readManager = readManager;
            _readClient = readClient;
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
}
