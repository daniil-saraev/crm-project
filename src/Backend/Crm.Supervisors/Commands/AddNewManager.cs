using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;

namespace Crm.Supervisors.Commands
{
    public record AddNewManagerRequest(
        Guid SupervisorId,
        Guid ManagerAccountId);

    public interface IAddNewManager
    {
        Task<Result> Execute(AddNewManagerRequest request, CancellationToken cancellationToken);
    }

    internal class AddNewManagerHandler : IAddNewManager
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;

        public AddNewManagerHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
        }

        public async Task<Result> Execute(AddNewManagerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var supervisor = await GetSupervisorById(request.SupervisorId, cancellationToken);
                supervisor.AddNewManager(request.ManagerAccountId);
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

        private async Task<Supervisor> GetSupervisorById(Guid id, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorByIdQuery(id),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(id.ToString(), nameof(Supervisor));
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
