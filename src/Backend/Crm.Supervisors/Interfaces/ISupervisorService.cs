using Ardalis.Result;

namespace Crm.Supervisors.Interfaces
{
    public interface ISupervisorService
    {
        Task<Result> AddNewManager(AddNewManager request, CancellationToken cancellationToken);

        Task<Result> AssignOrder(AssignOrder request, CancellationToken cancellationToken);

        Task<Result> TransferManager(TransferManager request, CancellationToken cancellationToken);
    }

    public record AddNewManager(
        Guid SupervisorId,
        Guid ManagerAccountId);

    public record AssignOrder(
        Guid SupervisorId,
        Guid ManagerId,
        Guid CreatedOrderId);

    public record TransferManager(
        Guid SupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId);
}
