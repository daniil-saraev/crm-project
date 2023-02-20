using Ardalis.Result;
using Crm.Clients.Queries;
using Crm.Core.Clients;
using Crm.Core.Clients.Events;
using Crm.Shared.Events;
using Crm.Shared.Models;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Clients.Commands
{
    public record CreateOrderRequest(
        string Name,
        string Email,
        string PhoneNumber,
        string Description) : IRequest<Result>;

    internal class CreateOrderHandler : IRequestHandler<CreateOrderRequest, Result>
    {
        private readonly IReadRepository<Client> _readRepository;
        private readonly IWriteRepository<Client> _writeRepository;
        private readonly IEventBus _eventBus;

        public CreateOrderHandler(IReadRepository<Client> readRepository, IWriteRepository<Client> writeRepository, IEventBus eventBus)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var client = await GetClientByPhoneNumber(request.PhoneNumber, cancellationToken);
            client ??= new Client(request.Name, new ContactInfo(request.Email, request.PhoneNumber));
            var order = client.PlaceOrder(request.Description);
            await _writeRepository.Update(client, cancellationToken);
            await _writeRepository.SaveChanges(cancellationToken);
            await _eventBus.Publish(new OrderCreatedEvent(client.Id, order.Id, client.ManagerId));
            return Result.Success();
        }

        private async Task<Client?> GetClientByPhoneNumber(string phoneNumber, CancellationToken cancellationToken)
        {
            return await _readRepository.Execute(
                    new ClientByPhoneNumberQuery(phoneNumber),
                    cancellationToken);
        }
    }
}
