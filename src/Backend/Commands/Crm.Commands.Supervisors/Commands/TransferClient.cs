using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Supervisors.Commands
{
    public record TransferClientCommand(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId) : ICommand;

    public record SupervisorWithManagersAndClientQuery(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId) : ISingleQuery<Supervisor>;

    internal class TransferClientHandler : IConsumer<TransferClientCommand>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IMessageBus _eventBus;

        public TransferClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IMessageBus eventBus)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _eventBus = eventBus;
        }

        public async Task Consume(ConsumeContext<TransferClientCommand> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        private async Task<Result> Handle(TransferClientCommand request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagersAndClient(
                    request.SupervisorId,
                    request.FromManagerId,
                    request.ToManagerId,
                    request.ClientId,
                    cancellationToken);
            supervisor.TransferClient(request.FromManagerId, request.ToManagerId, request.ClientId);
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewClientAssignedEvent(request.ToManagerId, request.ClientId), cancellationToken);
            return Result.Success();
        }

        private async Task<Supervisor> GetSupervisorWithManagersAndClient(
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
    }
}
