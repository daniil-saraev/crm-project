using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Clients;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;
using MediatR;

namespace Crm.Supervisors.Commands
{
    public record AssignClientRequest(
        Guid SupervisorId,
        Guid ManagerId,
        Guid ClientId) : IRequest<Result>;

    internal class AssignClientHandler : IRequestHandler<AssignClientRequest, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IReadRepository<Client> _readClient;

        public AssignClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IReadRepository<Client> readClient)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _readClient = readClient;
        }

        public async Task<Result> Handle(AssignClientRequest request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagers(request.SupervisorId, cancellationToken);
            var client = await GetClientWithOrders(request.ClientId, cancellationToken);
            supervisor.AssignClient(request.ManagerId, client);
            return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
        }

        private async Task<Supervisor> GetSupervisorWithManagers(Guid supervisorId, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagersQuery(supervisorId),
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

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }
}
