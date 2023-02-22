using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Supervisors.Commands
{
    public record TransferClientRequest(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId) : IRequest<Result>;

    internal class TransferClientHandler : IRequestHandler<TransferClientRequest, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;

        public TransferClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
        }

        public async Task<Result> Handle(TransferClientRequest request, CancellationToken cancellationToken)
        {
            var supervisor = await GetSupervisorWithManagersAndClient(
                    request.SupervisorId,
                    request.FromManagerId,
                    request.ToManagerId,
                    request.ClientId,
                    cancellationToken);
            supervisor.TransferClient(request.FromManagerId, request.ToManagerId, request.ClientId);
            return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
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

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }

    file record SupervisorWithManagersAndClientQuery(
        Guid SupervisorId,
        Guid FromManagerId,
        Guid ToManagerId,
        Guid ClientId) : ISingleQuery<Supervisor>;

    file class SupervisorWithManagersAndClientHandler : ISingleQueryHandler<SupervisorWithManagersAndClientQuery, Supervisor>
    {
        private readonly DbContext _dbContext;

        public SupervisorWithManagersAndClientHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Supervisor?> Handle(SupervisorWithManagersAndClientQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Supervisor>()
                .Where(supervisor => supervisor.Id == request.SupervisorId)
                .IncludeFilter(supervisor => supervisor.Managers
                    .Where(manager => manager.Id == request.FromManagerId || manager.Id == request.ToManagerId))

                .IncludeFilter(supervisor => supervisor.Managers
                    .Where(manager => manager.Id == request.FromManagerId)
                        .Select(manager => manager.Clients.Where(client => client.Id == request.ClientId)))

                .IncludeFilter(supervisor => supervisor.Managers
                    .Where(manager => manager.Id == request.FromManagerId)
                        .Select(manager => manager.Clients.Where(client => client.Id == request.ClientId)
                            .Select(client => client.OrdersInWork.AsEnumerable())))

                .IncludeFilter(supervisor => supervisor.Managers
                    .Where(manager => manager.Id == request.FromManagerId)
                        .Select(manager => manager.Clients.Where(client => client.Id == request.ClientId)
                            .Select(client => client.CreatedOrders.AsEnumerable())))

                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
