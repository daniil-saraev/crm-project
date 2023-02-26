using Ardalis.GuardClauses;
using Crm.Commands.Core.Managers;
using Crm.Messages.Clients;
using Crm.Messages.Managers;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Managers.EventHandlers
{
    public record ManagerWithClientWithCreatedOrdersQuery(
        Guid ManagerId,
        Guid ClientId) : ISingleQuery<Manager>;

    internal class ExistingClientPlacedOrderHandler : IConsumer<ExistingClientPlacedOrderEvent>
    {
        private readonly IWriteRepository<Manager> _writeManager;
        private readonly IReadRepository<Manager> _readManager;
        private readonly IMessageBus _eventBus;

        public ExistingClientPlacedOrderHandler(
            IWriteRepository<Manager> writeManager,
            IReadRepository<Manager> readManager,
            IMessageBus eventBus)
        {
            _writeManager = writeManager;
            _readManager = readManager;
            _eventBus = eventBus;
        }

        public async Task Consume(ConsumeContext<ExistingClientPlacedOrderEvent> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        private async Task Handle(ExistingClientPlacedOrderEvent @event, CancellationToken cancellationToken)
        {
            var manager = await GetManager(@event.ManagerId, @event.ClientId, cancellationToken);
            manager.TakeOrder(@event.CreatedOrderId, @event.ClientId);
            await _writeManager.Update(manager, cancellationToken);
            await _writeManager.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewOrderAssignedEvent(manager.Id, @event.CreatedOrderId), cancellationToken);
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
