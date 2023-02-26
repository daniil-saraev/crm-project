using Crm.Shared.Messages;
using MassTransit;

namespace Crm.Messages.Bus
{
    internal class MessageBus : IMessageBus
    {
        private readonly IBus _bus;

        public MessageBus(IBus bus)
        {
            _bus = bus;
        }

        public async Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _bus.Publish(domainEvent, cancellationToken);
        }

        public async Task Send(ICommand command, CancellationToken cancellationToken)
        {
            await _bus.Send(command, cancellationToken);
        }
    }
}
