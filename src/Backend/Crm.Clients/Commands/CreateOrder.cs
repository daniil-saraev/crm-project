using Ardalis.Result;
using Crm.Core.Clients;
using Crm.Core.Clients.Events;
using Crm.Shared.Events;
using Crm.Shared.Models;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Clients.Commands
{
    public record CreateOrderRequest(
        string Name,
        string Email,
        string PhoneNumber,
        string Description) : IRequest<Result<Guid>>;

    internal class CreateOrderHandler : IRequestHandler<CreateOrderRequest, Result<Guid>>
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

        public async Task<Result<Guid>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var client = await GetClientByPhoneNumber(request.PhoneNumber, cancellationToken);
            client ??= new Client(request.Name, new ContactInfo(request.Email, request.PhoneNumber));
            var order = client.PlaceOrder(request.Description);
            await _writeRepository.Update(client, cancellationToken);
            await _writeRepository.SaveChanges(cancellationToken);
            await _eventBus.Publish(new OrderCreatedEvent(client.Id, order.Id, client.ManagerId), cancellationToken);
            return Result.Success(order.Id);
        }

        private async Task<Client?> GetClientByPhoneNumber(string phoneNumber, CancellationToken cancellationToken)
        {
            return await _readRepository.Execute(
                    new ClientByPhoneNumberQuery(phoneNumber),
                    cancellationToken);
        }
    }

    file record ClientByPhoneNumberQuery(
        string PhoneNumber) : ISingleQuery<Client>;

    file class ClientByPhoneNumberHandler : ISingleQueryHandler<ClientByPhoneNumberQuery, Client>
    {
        private readonly DbContext _dataContext;

        public ClientByPhoneNumberHandler(DbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Client?> Handle(ClientByPhoneNumberQuery request, CancellationToken cancellationToken)
        {
            return await _dataContext.Set<Client>().FirstOrDefaultAsync(
                client => client.ContactInfo.PhoneNumber == request.PhoneNumber,
                cancellationToken);
        }
    }
}
