using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Extentions;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Supervisors.Commands
{
    public record TransferManagerCommand(
        string FromSupervisorId,
        string ToSupervisorId,
        string ManagerId) : IRequest<Result>;

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
            (Supervisor fromSupervisor, Supervisor toSupervisor) = await GetSupervisorsWithManager(
                request.FromSupervisorId.ToGuid(), 
                request.ToSupervisorId.ToGuid(), 
                request.ManagerId.ToGuid(), 
                cancellationToken);
            fromSupervisor.TransferManager(request.ManagerId.ToGuid(), toSupervisor);
            await _writeSupervisor.Update(fromSupervisor, cancellationToken);
            await _writeSupervisor.Update(toSupervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewManagerAddedEvent(toSupervisor.Id, request.ManagerId.ToGuid()), cancellationToken);
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
