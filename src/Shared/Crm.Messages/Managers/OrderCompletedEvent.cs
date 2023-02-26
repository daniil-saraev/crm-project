using Crm.Shared.Messages;

namespace Crm.Messages.Managers
{
    public record OrderCompletedEvent(
        Guid ManagerId,
        Guid CompletedOrderId) : IDomainEvent;
}
