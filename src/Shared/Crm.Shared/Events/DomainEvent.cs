using MediatR;

namespace Crm.Shared.Events
{
    public abstract record DomainEvent : INotification
    {

    }
}
