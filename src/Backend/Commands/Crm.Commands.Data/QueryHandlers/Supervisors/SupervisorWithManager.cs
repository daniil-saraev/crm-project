using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Supervisors
{
    internal class SupervisorWithManagerHandler : IRequestHandler<SupervisorWithManagerQuery, Supervisor?>
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
