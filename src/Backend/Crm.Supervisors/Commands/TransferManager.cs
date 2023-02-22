using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Supervisors.Commands
{
    public record TransferManagerRequest(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId) : IRequest<Result>;

    internal class TransferManagerHandler : IRequestHandler<TransferManagerRequest, Result>
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;

        public TransferManagerHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
        }

        public async Task<Result> Handle(TransferManagerRequest request, CancellationToken cancellationToken)
        {
            (Supervisor fromSupervisor, Supervisor toSupervisor)
                = await GetSupervisorsWithManager(request.FromSupervisorId, request.ToSupervisorId, request.ManagerId, cancellationToken);
            fromSupervisor.TransferManager(request.ManagerId, toSupervisor);
            await _writeSupervisor.Update(fromSupervisor, cancellationToken);
            await _writeSupervisor.Update(toSupervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
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

    file record SupervisorsWithManagerQuery(
        Guid FromSupervisorId,
        Guid ToSupervisorId,
        Guid ManagerId) : ICollectionQuery<Supervisor>;

    file class SupervisorsWithManagerHandler : ICollectionQueryHandler<SupervisorsWithManagerQuery, Supervisor>
    {
        private readonly DbContext _dbContext;

        public SupervisorsWithManagerHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Supervisor>> Handle(SupervisorsWithManagerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Supervisor>()
                .Where(supervisor => supervisor.Id == request.FromSupervisorId || supervisor.Id == request.ToSupervisorId)
                .IncludeFilter(supervisor => supervisor.Managers.Where(manager => manager.Id == request.ManagerId))
                .ToListAsync(cancellationToken);
        }
    }
}
