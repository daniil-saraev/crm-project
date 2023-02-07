using Crm.Shared.Interfaces;
using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Services
{
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent)
        {
            await _mediator.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}
