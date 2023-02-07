using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Interfaces
{
    public interface IEventHandler<T> : INotificationHandler<T> where T: DomainEvent
    {
    }
}
