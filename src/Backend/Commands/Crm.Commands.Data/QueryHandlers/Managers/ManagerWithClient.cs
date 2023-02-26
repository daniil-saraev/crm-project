using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands.Shared;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Commands.Data.QueryHandlers.Managers
{
    internal class ManagerWithClientHandler : ISingleQueryHandler<ManagerWithClientQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerWithClientHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerWithClientQuery request, CancellationToken cancellationToken)
        {
            return await _context.Set<Manager>()
                .Where(manager => manager.Id == request.ManagerId)
                .IncludeFilter(manager => manager.Clients
                    .Where(client => client.Id == request.ClientId))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
