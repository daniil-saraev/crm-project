using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Commands.Data.QueryHandlers.Supervisors
{
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
