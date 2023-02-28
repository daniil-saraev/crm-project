using Crm.Commands.Core.Supervisors;
using Crm.Commands.Supervisors.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Supervisors
{
    file class SupervisorWithManagersAndClientHandler : IRequestHandler<SupervisorWithManagersAndClientQuery, Supervisor?>
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
