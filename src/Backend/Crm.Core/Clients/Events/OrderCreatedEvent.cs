using Crm.Shared.Events;

namespace Crm.Core.Clients.Events
{
    public record OrderCreatedEvent(
        Guid ClientId,
        Guid OrderId,
        Guid? ManagerId) : DomainEvent;
}
