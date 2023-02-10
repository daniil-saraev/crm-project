using Ardalis.Result;
using Crm.Clients.Interfaces;
using Crm.Clients.Specifications;
using Crm.Core.Interfaces;
using Crm.Core.Models.Clients;
using Crm.Clients.Contracts;
using Crm.Shared.Models;

namespace Crm.Clients.Services
{
    internal class CreateOrderService : ICreateOrderService
    {
        private readonly IRepository<Client> _repository;

        public CreateOrderService(IRepository<Client> repository)
        {
            _repository = repository;
        }
        
        public async Task<Result> CreateOrder(CreateOrder request, CancellationToken cancellationToken)
        {
            try
            {
                var client = await _repository.FirstOrDefaultAsync(new ClientByPhoneNumber(request.PhoneNumber), cancellationToken);
                client ??= new Client(request.Name, new ContactInfo(request.Email, request.PhoneNumber));
                client.PlaceOrder(request.Description);
                await _repository.UpdateAsync(client, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }
    }
}
