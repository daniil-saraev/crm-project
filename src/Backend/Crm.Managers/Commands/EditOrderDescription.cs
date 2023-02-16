using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Managers.Queries;
using Crm.Shared.Repository;

namespace Crm.Managers.Commands
{
    public record EditOrderDescriptionRequest(
    Guid ManagerId,
    Guid OrderInWorkId,
    string Description);

    public interface IEditOrderDescription
    {
        Task<Result> Execute(EditOrderDescriptionRequest request, CancellationToken cancellationToken);
    }

    internal class EditOrderDescriptionHandler : IEditOrderDescription
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditOrderDescriptionHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result> Execute(EditOrderDescriptionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await GetManagerWithOrder(request.ManagerId, request.OrderInWorkId, cancellationToken);
                manager.SetOrderDescription(request.OrderInWorkId, request.Description);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
            }
            catch (NotFoundException ex)
            {
                return Result.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }

        private async Task<Manager> GetManagerWithOrder(Guid managerId, Guid orderInWorkId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
            new ManagerWithOrderInWorkQuery(managerId, orderInWorkId),
                    cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }

        private async Task<Result> SaveChangesAndReturnSuccess(Manager manager, CancellationToken cancellationToken)
        {
            await _writeManager.Update(manager, cancellationToken);
            await _writeManager.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }
}
