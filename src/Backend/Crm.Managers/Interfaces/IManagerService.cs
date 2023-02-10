using Ardalis.Result;
using Crm.Managers.Contracts;

namespace Crm.Managers.Interfaces
{
    public interface IManagerService
    {
        Task<Result> CompleteOrder(CompleteOrder request, CancellationToken cancellationToken);
        Task<Result> EditOrderDescription(EditOrderDescription request, CancellationToken cancellationToken);
        Task<Result> EditClientName(EditClientName request, CancellationToken cancellationToken);
        Task<Result> EditClientContactInfo(EditClientInfo request, CancellationToken cancellationToken);
    }
}
