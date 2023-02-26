using Crm.Shared.Messages;

namespace Crm.Messages.Clients
{
    public record ExistingClientPlacedOrderEvent(
        Guid ClientId,
        Guid CreatedOrderId,
        Guid ManagerId) : IDomainEvent;
}
