using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Supervisors.Commands
{
    public record AssignClientCommand(
        Guid SupervisorId,
        Guid ManagerId,
        Guid ClientId) : ICommand;

    public record SupervisorWithManagerQuery(
        Guid SupervisorId,
        Guid ManagerId) : ISingleQuery<Supervisor>;

    public record ClientWithOrdersQuery(
        Guid ClientId) : ISingleQuery<Client>;

    internal class AssignClientHandler : IConsumer<AssignClientCommand>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IReadRepository<Client> _readClient;
        private readonly IMessageBus _eventBus;

        public AssignClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IReadRepository<Client> readClient,
            IMessageBus eventBus)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _readClient = readClient;
            _eventBus = eventBus;
        }

        public async Task Consume(ConsumeContext<AssignClientCommand> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        public async Task<Result> Handle(AssignClientCommand request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManager(request.SupervisorId, request.ManagerId, cancellationToken);
            var client = await GetClientWithOrders(request.ClientId, cancellationToken);
            supervisor.AssignClient(request.ManagerId, client);
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewClientAssignedEvent(request.ManagerId, client.Id), cancellationToken);
            return Result.Success();
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

        private async Task<Client> GetClientWithOrders(Guid id, CancellationToken cancellationToken)
        {
            var client = await _readClient.Execute(
                    new ClientWithOrdersQuery(id),
                    cancellationToken);
            if (client == null)
                throw new NotFoundException(id.ToString(), nameof(Client));
            return client;
        }
    }
}
