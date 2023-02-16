using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;

namespace Crm.Supervisors.Commands
{
    public record TransferClientRequest(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId);

    public interface ITransferClient
    {
        Task<Result> TransferClient(TransferClientRequest request, CancellationToken cancellationToken);
    }

    internal class TransferClientHandler : ITransferClient
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

        public async Task<Result> TransferClient(TransferClientRequest request, CancellationToken cancellationToken)
        {
            try
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
            catch (NotFoundException ex)
            {
                return Result.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
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
