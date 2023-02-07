using Crm.Shared.Models;

namespace Crm.Shared.Interfaces
{
    public interface IEventBus
    {
        Task Publish(DomainEvent domainEvent);
    }
}
