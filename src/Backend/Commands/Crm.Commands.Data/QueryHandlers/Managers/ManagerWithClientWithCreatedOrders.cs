using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.EventHandlers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Managers
{
    internal class ManagerWithClientWithCreatedOrdersHandler : ISingleQueryHandler<ManagerWithClientWithCreatedOrdersQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerWithClientWithCreatedOrdersHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerWithClientWithCreatedOrdersQuery request, CancellationToken cancellationToken)
        {
            var manager = await _context.Set<Manager>()
                    .Where(manager => manager.Id == request.ManagerId)
                    .IncludeFilter(manager => manager.Clients
                        .Where(client => client.Id == request.ClientId))
                    .IncludeFilter(manager => manager.Clients
                        .Where(client => client.Id == request.ClientId)
                            .Select(client => client.CreatedOrders))
                    .SingleOrDefaultAsync(cancellationToken);
            return manager;
        }
    }
}
