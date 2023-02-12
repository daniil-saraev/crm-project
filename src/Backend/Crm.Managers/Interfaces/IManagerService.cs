using Ardalis.Result;
using static Crm.Core.Orders.CompletedOrder;

namespace Crm.Managers.Interfaces
{
    public interface IManagerService
    {
        Task<Result> CompleteOrder(CompleteOrder request, CancellationToken cancellationToken);
        Task<Result> EditOrderDescription(EditOrderDescription request, CancellationToken cancellationToken);
        Task<Result> EditClientName(EditClientName request, CancellationToken cancellationToken);
        Task<Result> EditClientContactInfo(EditClientInfo request, CancellationToken cancellationToken);
    }

    public record CompleteOrder(
    Guid ManagerId,
    Guid OrderInWorkId,
    CompletionStatus Status,
    string Comment);

    public record EditClientInfo(
    Guid ManagerId,
    Guid ClientId,
    string Email,
    string PhoneNumber);

    public record EditClientName(
    Guid ManagerId,
    Guid ClientId,
    string Name);

    public record EditOrderDescription(
    Guid ManagerId,
    Guid OrderInWorkId,
    string Description);
}
