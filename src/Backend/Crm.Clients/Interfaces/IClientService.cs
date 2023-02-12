using Ardalis.Result;

namespace Crm.Clients.Interfaces
{
    public interface IClientService
    {
        Task<Result> CreateOrder(CreateOrder request, CancellationToken cancellationToken);
    }

    public record CreateOrder(
        string Name,
        string Email,
        string PhoneNumber,
        string Description);
}
