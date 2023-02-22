using Crm.Core.Managers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.Commands.Shared
{
    internal record ManagerWithClientQuery(
        Guid ManagerId,
        Guid ClientId) : ISingleQuery<Manager>;

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
