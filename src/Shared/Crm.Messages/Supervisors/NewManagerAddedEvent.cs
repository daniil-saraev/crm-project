using Crm.Shared.Messages;

namespace Crm.Messages.Supervisors
{
    public record NewManagerAddedEvent(
        Guid SupervisorId,
        Guid ManagerId) : IDomainEvent;
}
