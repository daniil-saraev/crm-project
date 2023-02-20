using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;
using MediatR;

namespace Crm.Supervisors.Commands
{
    public record TransferManagerRequest(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId) : IRequest<Result>;

    internal class TransferManagerHandler : IRequestHandler<TransferManagerRequest, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;

        public TransferManagerHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
        }

        public async Task<Result> Handle(TransferManagerRequest request, CancellationToken cancellationToken)
        {
            var fromSupervisor = await GetSupervisorWithManagers(request.FromSupervisorId, cancellationToken);
            var toSupervisor = await GetSupervisor(request.ToSupervisorId, cancellationToken);
            fromSupervisor.TransferManager(request.ManagerId, toSupervisor);
            await _writeSupervisor.Update(fromSupervisor, cancellationToken);
            await _writeSupervisor.Update(toSupervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }

        private async Task<Supervisor> GetSupervisor(Guid id, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorByIdQuery(id),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(id.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<Supervisor> GetSupervisorWithManagers(Guid supervisorId, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagersQuery(supervisorId),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(supervisorId.ToString(), nameof(Supervisor));
            return supervisor;
        }
    }
}
