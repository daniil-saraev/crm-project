using Crm.Commands.Core.Clients;
using Crm.Commands.Supervisors.Commands;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Commands.Data.QueryHandlers.Supervisors
{
    internal class ClientWithOrdersHandler : IRequestHandler<ClientWithOrdersQuery, Client?>
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
