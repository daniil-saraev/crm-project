using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Clients;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

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
            var supervisor = await GetSupervisorWithManager(request.SupervisorId, request.ManagerId, cancellationToken);
            var client = await GetClientWithOrders(request.ClientId, cancellationToken);
            supervisor.AssignClient(request.ManagerId, client);
            return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
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

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }

    file record SupervisorWithManagerQuery(
        Guid SupervisorId,
        Guid ManagerId) : ISingleQuery<Supervisor>;

    file class SupervisorWithManagerHandler : ISingleQueryHandler<SupervisorWithManagerQuery, Supervisor>
    {
        private readonly DbContext _dbContext;

        public SupervisorWithManagerHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Supervisor?> Handle(SupervisorWithManagerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Supervisor>()
                .Where(supervisor => supervisor.Id == request.SupervisorId)
                .IncludeFilter(supervisor => supervisor.Managers
                    .Where(manager => manager.Id == request.ManagerId))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

    file record ClientWithOrdersQuery(
        Guid ClientId) : ISingleQuery<Client>;

    file class ClientWithOrdersHandler : ISingleQueryHandler<ClientWithOrdersQuery, Client>
    {
        private readonly DbContext _dbContext;

        public ClientWithOrdersHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Client?> Handle(ClientWithOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Client>()
                .Where(client => client.Id == request.ClientId)
                .Include(client => client.CreatedOrders)
                .Include(client => client.OrdersInWork)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
