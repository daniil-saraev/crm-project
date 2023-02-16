using Crm.Core.Clients;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Supervisors.Queries
{
    internal record ClientWithOrdersQuery(
        Guid ClientId) : ISingleQuery<Client>;

    internal class ClientWithOrdersHandler : ISingleQueryHandler<ClientWithOrdersQuery, Client>
    {
        private readonly DbContext _dbContext;

        public ClientWithOrdersHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Client?> Handle(ClientWithOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Client>()
                .Where(client => client.Id == request.ClientId)
                .Include(client => client.CreatedOrders)
                .Include(client => client.OrdersInWork)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
