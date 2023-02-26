using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Managers
{
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
