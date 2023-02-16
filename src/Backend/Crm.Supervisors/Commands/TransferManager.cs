using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;

namespace Crm.Supervisors.Commands
{
    public record TransferManagerRequest(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId);

    public interface ITransferManager
    {
        Task<Result> Execute(TransferManagerRequest request, CancellationToken cancellationToken);
    }

    internal class TransferManagerHandler : ITransferManager
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

        public async Task<Result> Execute(TransferManagerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var fromSupervisor = await GetSupervisorWithManager(request.FromSupervisorId, request.ManagerId, cancellationToken);
                var toSupervisor = await GetSupervisor(request.ToSupervisorId, cancellationToken);
                fromSupervisor.TransferManager(request.ManagerId, toSupervisor);
                await _writeSupervisor.Update(fromSupervisor, cancellationToken);
                await _writeSupervisor.Update(toSupervisor, cancellationToken);
                await _writeSupervisor.SaveChanges(cancellationToken);
                return Result.Success();
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

        private async Task<Supervisor> GetSupervisor(Guid id, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorByIdQuery(id),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(id.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<Supervisor> GetSupervisorWithManager(Guid supervisorId, Guid managerId, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagerQuery(supervisorId, managerId),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(supervisorId.ToString(), nameof(Supervisor));
            return supervisor;
        }
    }
}
