using MediatR;

namespace Crm.Shared.Events
{
    public interface IEventHandler<T> : INotificationHandler<T> where T : DomainEvent
    {
    }
}
