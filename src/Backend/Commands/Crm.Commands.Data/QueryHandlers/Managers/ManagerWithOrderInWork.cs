using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Managers
{
    internal class ManagerWithOrderInWorkHandler : ISingleQueryHandler<ManagerWithOrderInWorkQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerWithOrderInWorkHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerWithOrderInWorkQuery request, CancellationToken cancellationToken)
        {
            var manager = await _context.Set<Manager>()
                    .Where(manager => manager.Id == request.ManagerId)
                    .IncludeFilter(manager => manager.OrdersInWork
                        .Where(order => order.Id == request.OrderInWorkId))
                    .SingleOrDefaultAsync(cancellationToken);
            return manager;
        }
    }
}
