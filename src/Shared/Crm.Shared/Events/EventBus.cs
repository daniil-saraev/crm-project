using MediatR;

namespace Crm.Shared.Events
{
    internal class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
