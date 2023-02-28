using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Supervisors
{
    internal class SupervisorsWithManagerHandler : IRequestHandler<SupervisorsWithManagerQuery, IEnumerable<Supervisor>>
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
