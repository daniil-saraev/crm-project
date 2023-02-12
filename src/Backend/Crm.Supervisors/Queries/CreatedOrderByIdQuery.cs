using Crm.Core.Orders;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Supervisors.Queries
{
    internal record CreatedOrderByIdQuery(
        Guid CreatedOrderId) : ISingleQuery<CreatedOrder>;

    internal class CreatedOrderByIdHandler : ISingleQueryHandler<CreatedOrderByIdQuery, CreatedOrder>
    {
        private readonly DbContext _dbContext;

        public CreatedOrderByIdHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreatedOrder?> Handle(CreatedOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<CreatedOrder>().FirstOrDefaultAsync(
                order => order.Id == request.CreatedOrderId,
                cancellationToken);
        }
    }
}
