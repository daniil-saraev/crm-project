using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Supervisors.Commands
{
    public record TransferManagerCommand(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId) : IRequest<Result>;

    public record SupervisorsWithManagerQuery(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId) : ICollectionQuery<Supervisor>;

    internal class TransferManagerHandler : IRequestHandler<TransferManagerCommand, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IMessageBus _eventBus;

        public TransferManagerHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IMessageBus eventBus)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(TransferManagerCommand request, CancellationToken cancellationToken)
        {
            (Supervisor fromSupervisor, Supervisor toSupervisor)
                = await GetSupervisorsWithManager(request.FromSupervisorId, request.ToSupervisorId, request.ManagerId, cancellationToken);
            fromSupervisor.TransferManager(request.ManagerId, toSupervisor);
            await _writeSupervisor.Update(fromSupervisor, cancellationToken);
            await _writeSupervisor.Update(toSupervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewManagerAddedEvent(toSupervisor.Id, request.ManagerId), cancellationToken);
            return Result.Success();
        }

        private async Task<(Supervisor fromSupervisor, Supervisor toSupervisor)> GetSupervisorsWithManager(
            Guid fromSupervisorId, Guid toSupervisorId, Guid managerId, CancellationToken cancellationToken)
        {
            var supervisors = await _readSupervisor.Execute(
                new SupervisorsWithManagerQuery(fromSupervisorId, toSupervisorId, managerId),
                cancellationToken);

            var fromSupervisor = supervisors.FirstOrDefault(s => s.Id == fromSupervisorId);
            if (fromSupervisor == null)
                throw new NotFoundException(fromSupervisorId.ToString(), nameof(Supervisor));

            var toSupervisor = supervisors.FirstOrDefault(s => s.Id == toSupervisorId);
            if (toSupervisor == null)
                throw new NotFoundException(toSupervisorId.ToString(), nameof(Supervisor));

            return (fromSupervisor, toSupervisor);
        }
    }
}
