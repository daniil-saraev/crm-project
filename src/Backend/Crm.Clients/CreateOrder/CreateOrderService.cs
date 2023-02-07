using Ardalis.Result;
using Crm.Clients.Specifications;
using Crm.Core.Interfaces;
using Crm.Core.Models.Clients;
using Crm.Core.Models.Orders;
using Crm.Dto.Clients;
using Crm.Dto.Orders;
using Crm.Shared.Models;

namespace Crm.Clients.CreateOrder
{
    internal class CreateOrderService : ICreateOrderService
    {
        private readonly IRepository<Client> _repository;

        public CreateOrderService(IRepository<Client> repository)
        {
            _repository = repository;
        }

        public async Task<Result> CreateOrder(ClientDto clientDto, OrderDto orderDto, CancellationToken cancellationToken)
        {
            try
            {
                var client = await _repository.FirstOrDefaultAsync(new GetByPhoneNumber(clientDto.PhoneNumber), cancellationToken);
                client ??= new Client(clientDto.Name, new ContactInfo(clientDto.Email, clientDto.PhoneNumber));
                client.CreatedOrders.Add(new CreatedOrder(client.Id));
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
