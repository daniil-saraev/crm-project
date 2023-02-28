using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Supervisors.Commands
{
    public record AddNewManagerCommand(
        Guid SupervisorId,
        Guid ManagerAccountId) : IRequest<Result>;

    public record SupervisorWithManagersQuery(
        Guid SupervisorId) : ISingleQuery<Supervisor>;

    internal class AddNewManagerHandler : IRequestHandler<AddNewManagerCommand, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IMessageBus _eventBus;

        public AddNewManagerHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IMessageBus eventBus)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(AddNewManagerCommand request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagers(request.SupervisorId, cancellationToken);
            var manager = supervisor.AddNewManager(request.ManagerAccountId);
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewManagerAddedEvent(supervisor.Id, manager.Id), cancellationToken);
            return Result.Success();
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
    }
}
