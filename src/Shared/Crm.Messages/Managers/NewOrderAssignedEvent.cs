using Crm.Shared.Messages;

namespace Crm.Messages.Managers
{
    public record NewOrderAssignedEvent(
        Guid ManagerId,
        Guid OrderId) : IDomainEvent;
}
