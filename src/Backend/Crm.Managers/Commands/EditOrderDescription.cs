using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Managers;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.Commands
{
    public record EditOrderDescriptionRequest(
    Guid ManagerId,
    Guid OrderInWorkId,
    string Description) : IRequest<Result>;

    internal class EditOrderDescriptionHandler : IRequestHandler<EditOrderDescriptionRequest, Result>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditOrderDescriptionHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result> Handle(EditOrderDescriptionRequest request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithOrder(request.ManagerId, request.OrderInWorkId, cancellationToken);
            manager.SetOrderDescription(request.OrderInWorkId, request.Description);
            return await SaveChangesAndReturnSuccess(manager, cancellationToken);
        }

        private async Task<Manager> GetManagerWithOrder(Guid managerId, Guid orderInWorkId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
            new ManagerWithOrderInWorkQuery(managerId, orderInWorkId),
                    cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }

        private async Task<Result> SaveChangesAndReturnSuccess(Manager manager, CancellationToken cancellationToken)
        {
            await _writeManager.Update(manager, cancellationToken);
            await _writeManager.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }

    file record ManagerWithOrderInWorkQuery(
    Guid ManagerId,
    Guid OrderInWorkId) : ISingleQuery<Manager>;

    file class ManagerWithOrderInWorkHandler : ISingleQueryHandler<ManagerWithOrderInWorkQuery, Manager>
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
