using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;
using MediatR;

namespace Crm.Supervisors.Commands
{
    public record AddNewManagerRequest(
        Guid SupervisorId,
        Guid ManagerAccountId) : IRequest<Result>;

    internal class AddNewManagerHandler : IRequestHandler<AddNewManagerRequest, Result>
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

        public async Task<Result> Handle(AddNewManagerRequest request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagers(request.SupervisorId, cancellationToken);
            supervisor.AddNewManager(request.ManagerAccountId);
            return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
        }

        private async Task<Supervisor> GetSupervisorWithManagers(Guid id, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagersQuery(id),
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
