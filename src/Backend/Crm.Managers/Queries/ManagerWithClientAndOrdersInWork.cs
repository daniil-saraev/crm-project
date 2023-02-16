using Crm.Core.Managers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.Queries
{
    internal record ManagerWithClientAndOrdersInWorkQuery(
    Guid ManagerId,
    Guid ClientId,
    Guid OrderInWorkId) : ISingleQuery<Manager>;

    internal class ManagerWithClientAndOrdersInWorkHandler : ISingleQueryHandler<ManagerWithClientAndOrdersInWorkQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerWithClientAndOrdersInWorkHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerWithClientAndOrdersInWorkQuery request, CancellationToken cancellationToken)
        {
            return await _context.Set<Manager>()
                .Where(manager => manager.Id == request.ManagerId)
                .IncludeFilter(manager => manager.Clients
                    .Where(client => client.Id == request.ClientId))
                .IncludeFilter(manager => manager.Clients
                    .Where(client => client.Id == request.ClientId)
                        .Select(client => client.OrdersInWork
                            .Where(order => order.Id == request.OrderInWorkId)))
                .IncludeFilter(manager => manager.OrdersInWork
                    .Where(order => order.Id == request.OrderInWorkId))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}