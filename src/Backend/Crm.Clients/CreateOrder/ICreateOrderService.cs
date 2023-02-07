using Ardalis.Result;
using Crm.Dto.Clients;
using Crm.Dto.Orders;

namespace Crm.Clients.CreateOrder
{
    public interface ICreateOrderService
    {
        Task<Result> CreateOrder(ClientDto client, OrderDto order, CancellationToken cancellationToken);
    }
}
