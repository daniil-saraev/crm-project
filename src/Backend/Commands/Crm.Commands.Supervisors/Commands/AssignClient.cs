using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Extentions;
using Crm.Commands.Core.Supervisors;
using Crm.Messages.Supervisors;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Supervisors.Commands
{
    public record AssignClientCommand(
        string SupervisorId,
        string ManagerId,
        string ClientId) : IRequest<Result>;

    public record SupervisorWithManagerQuery(
        Guid SupervisorId,
        Guid ManagerId) : ISingleQuery<Supervisor>;

    public record ClientWithOrdersQuery(
        Guid ClientId) : ISingleQuery<Client>;

    internal class AssignClientHandler : IRequestHandler<AssignClientCommand, Result>
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

        public async Task<Result> Handle(AssignClientCommand request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManager(request.SupervisorId.ToGuid(), request.ManagerId.ToGuid(), cancellationToken);
            var client = await GetClientWithOrders(request.ClientId.ToGuid(), cancellationToken);
            supervisor.AssignClient(request.ManagerId.ToGuid(), client);
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            await _eventBus.Publish(new NewClientAssignedEvent(request.ManagerId.ToGuid(), client.Id), cancellationToken);
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
