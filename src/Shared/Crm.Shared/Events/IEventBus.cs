namespace Crm.Shared.Events
{
    public interface IEventBus
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
