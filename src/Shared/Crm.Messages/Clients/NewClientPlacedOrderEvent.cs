using Crm.Shared.Messages;

namespace Crm.Messages.Clients
{
    public record NewClientPlacedOrderEvent(
        Guid ClientId,
        Guid CreatedOrderId) : IDomainEvent;
}
