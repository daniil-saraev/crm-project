using Crm.Shared.Messages;

namespace Crm.Messages.Supervisors
{
    public record NewClientAssignedEvent(
        Guid ManagerId,
        Guid ClientId) : IDomainEvent;
}