using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;
using MediatR;

namespace Crm.Supervisors.Commands
{
    public record TransferClientRequest(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId) : IRequest<Result>;

    internal class TransferClientHandler : IRequestHandler<TransferClientRequest, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;

        public TransferClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
        }

        public async Task<Result> Handle(TransferClientRequest request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagers(
                    request.SupervisorId,
                    request.FromManagerId,
                    request.ToManagerId,
                    request.ClientId,
                    cancellationToken);
            supervisor.TransferClient(request.FromManagerId, request.ToManagerId, request.ClientId);
            return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
        }

        private async Task<Supervisor> GetSupervisorWithManagers(
            Guid supervisorId, 
            Guid fromManagerId, 
            Guid toManagerId, 
            Guid clientId, 
            CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagersAndClientQuery(supervisorId, fromManagerId, toManagerId, clientId),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(supervisorId.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }
}
