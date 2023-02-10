using Ardalis.Result;
using Crm.Clients.Contracts;

namespace Crm.Clients.Interfaces
{
    public interface ICreateOrderService
    {
        Task<Result> CreateOrder(CreateOrder request, CancellationToken cancellationToken);
    }
}
