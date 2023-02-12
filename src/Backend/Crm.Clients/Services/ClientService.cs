using Ardalis.Result;
using Crm.Clients.Interfaces;
using Crm.Shared.Models;
using Crm.Core.Clients;
using Crm.Shared.Repository;
using Crm.Clients.Queries;

namespace Crm.Clients.Services
{
    internal class ClientService : IClientService
    {
        private readonly IReadRepository<Client> _readRepository;
        private readonly IWriteRepository<Client> _writeRepository;

        public ClientService(IReadRepository<Client> readRepository, IWriteRepository<Client> writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }
        
        public async Task<Result> CreateOrder(CreateOrder request, CancellationToken cancellationToken)
        {
            try
            {
                var client = await _readRepository.Execute(
                    new ClientByPhoneNumberQuery(request.PhoneNumber),
                    cancellationToken);
                client ??= new Client(request.Name, new ContactInfo(request.Email, request.PhoneNumber));
                client.PlaceOrder(request.Description);
                await _writeRepository.Update(client, cancellationToken);
                await _writeRepository.SaveChanges(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }
    }
}
