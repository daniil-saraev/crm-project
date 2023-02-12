using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Supervisors.Queries
{
    internal record SupervisorByIdQuery(
        Guid SupervisorId) : ISingleQuery<Supervisor>;

    internal class SupervisorByIdHandler : ISingleQueryHandler<SupervisorByIdQuery, Supervisor>
    {
        private readonly DbContext _dbContext;

        public SupervisorByIdHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Supervisor?> Handle(SupervisorByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Supervisor>().FirstOrDefaultAsync(
                supervisor => supervisor.Id == request.SupervisorId,
                cancellationToken);
        }
    }
}
