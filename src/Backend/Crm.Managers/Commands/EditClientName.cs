using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Managers.Queries;
using Crm.Shared.Repository;

namespace Crm.Managers.Commands
{
    public record EditClientNameRequest(
    Guid ManagerId,
    Guid ClientId,
    string Name);

    public interface IEditClientName
    {
        Task<Result> EditClientName(EditClientNameRequest request, CancellationToken cancellationToken);
    }

    internal class EditClientNameHandler : IEditClientName
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditClientNameHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result> EditClientName(EditClientNameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await GetManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
                manager.SetClientName(request.ClientId, request.Name);
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

        private async Task<Manager> GetManagerWithClient(Guid managerId, Guid clientId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
                    new ManagerWithClientQuery(managerId, clientId),
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
