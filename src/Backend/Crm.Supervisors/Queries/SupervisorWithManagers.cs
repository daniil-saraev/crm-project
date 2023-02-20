using Crm.Core.Managers;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Supervisors.Queries
{
    internal record SupervisorWithManagersQuery(
        Guid SupervisorId) : ISingleQuery<Supervisor>;

    internal class SupervisorWithManagersHandler : ISingleQueryHandler<SupervisorWithManagersQuery, Supervisor>
    {
        private readonly DbContext _dbContext;

        public SupervisorWithManagersHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Supervisor?> Handle(SupervisorWithManagersQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Supervisor>()
                .Where(supervisor => supervisor.Id == request.SupervisorId)
                .Include(supervisor => supervisor.Managers)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
