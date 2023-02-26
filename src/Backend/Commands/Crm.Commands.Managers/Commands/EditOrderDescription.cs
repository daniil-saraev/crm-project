using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Managers;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Managers.Commands
{
    public record EditOrderDescriptionCommand(
    Guid ManagerId,
    Guid OrderInWorkId,
    string Description) : ICommand;

    public record ManagerWithOrderInWorkQuery(
    Guid ManagerId,
    Guid OrderInWorkId) : ISingleQuery<Manager>;

    internal class EditOrderDescriptionHandler : IConsumer<EditOrderDescriptionCommand>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditOrderDescriptionHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task Consume(ConsumeContext<EditOrderDescriptionCommand> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        private async Task<Result> Handle(EditOrderDescriptionCommand request, CancellationToken cancellationToken)
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
}
