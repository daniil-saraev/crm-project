using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Messages.Clients;
using Crm.Shared.Messages;
using Crm.Shared.Models;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Clients.Commands
{
    public record ClientByPhoneNumberQuery(
        string PhoneNumber) : ISingleQuery<Client>;

    internal class CreateOrderHandler : IConsumer<CreateOrderCommand>
    {
        private readonly IReadRepository<Client> _readRepository;
        private readonly IWriteRepository<Client> _writeRepository;
        private readonly IMessageBus _eventBus;

        public CreateOrderHandler(IReadRepository<Client> readRepository, IWriteRepository<Client> writeRepository, IMessageBus eventBus)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _eventBus = eventBus;
        }

        public async Task Consume(ConsumeContext<CreateOrderCommand> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        private async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var client = await GetClientByPhoneNumber(request.PhoneNumber, cancellationToken);
            client ??= new Client(request.Name, new ContactInfo(request.Email, request.PhoneNumber));
            var order = client.PlaceOrder(request.Description);
            await _writeRepository.Update(client, cancellationToken);
            await _writeRepository.SaveChanges(cancellationToken);
            if (client.ManagerId != null)
                await _eventBus.Publish(new ExistingClientPlacedOrderEvent(
                    client.Id, order.Id, client.ManagerId.Value), cancellationToken);
            else
                await _eventBus.Publish(new NewClientPlacedOrderEvent(
                    client.Id, order.Id), cancellationToken);

            return Result.Success(order.Id);
        }

        private async Task<Client?> GetClientByPhoneNumber(string phoneNumber, CancellationToken cancellationToken)
        {
            return await _readRepository.Execute(
                    new ClientByPhoneNumberQuery(phoneNumber),
                    cancellationToken);
        }
    }
}
