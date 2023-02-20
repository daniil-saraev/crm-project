using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Managers.Queries;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Managers.Commands
{
    public record EditClientNameRequest(
    Guid ManagerId,
    Guid ClientId,
    string Name) : IRequest<Result>;

    internal class EditClientNameHandler : IRequestHandler<EditClientNameRequest, Result>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditClientNameHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result> Handle(EditClientNameRequest request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
            manager.SetClientName(request.ClientId, request.Name);
            return await SaveChangesAndReturnSuccess(manager, cancellationToken);
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
