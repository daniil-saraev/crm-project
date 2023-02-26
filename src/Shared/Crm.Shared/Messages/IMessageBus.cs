namespace Crm.Shared.Messages
{
    public interface IMessageBus
    {
        Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken);

        Task Send(ICommand command, CancellationToken cancellationToken);
    }
}
