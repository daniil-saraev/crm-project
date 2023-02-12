using Crm.Core.Managers;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Managers.Queries
{
    internal record ManagerByIdQuery(
    Guid ManagerId) : ISingleQuery<Manager>;

    internal class ManagerByIdHandler : ISingleQueryHandler<ManagerByIdQuery, Manager>
    {
        private readonly DbContext _context;

        public ManagerByIdHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> Handle(ManagerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Set<Manager>().FirstOrDefaultAsync(
                manager => manager.Id == request.ManagerId,
                cancellationToken);
        }
    }
}