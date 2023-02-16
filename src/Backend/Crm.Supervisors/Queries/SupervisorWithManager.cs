using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Supervisors.Queries
{
    internal record SupervisorWithManagerQuery(
        Guid SupervisorId,
        Guid ManagerId) : ISingleQuery<Supervisor>;

    internal class SupervisorWithManagerHandler : ISingleQueryHandler<SupervisorWithManagerQuery, Supervisor>
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
}
