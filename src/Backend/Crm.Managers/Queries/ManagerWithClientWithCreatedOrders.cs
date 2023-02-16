using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.Queries
{
    internal record ManagerWithClientWithCreatedOrdersQuery(
        Guid ManagerId,
        Guid ClientId) : ISingleQuery<Manager>;

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
