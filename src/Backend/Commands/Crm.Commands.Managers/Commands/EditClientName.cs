using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands.Shared;
using Crm.Shared.Messages;
using Crm.Shared.Repository;
using MassTransit;

namespace Crm.Commands.Managers.Commands
{
    public record EditClientNameCommand(
    Guid ManagerId,
    Guid ClientId,
    string Name) : ICommand;

    internal class EditClientNameHandler : IConsumer<EditClientNameCommand>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditClientNameHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task Consume(ConsumeContext<EditClientNameCommand> context)
        {
            await Handle(context.Message, context.CancellationToken);
        }

        private async Task<Result> Handle(EditClientNameCommand request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
            manager.SetClientName(request.ClientId, request.Name);
            return await SaveChangesAndReturnSuccess(manager, cancellationToken);
        }

        private async Task<Manager> GetManagerWithClient(Guid managerId, Guid clientId, CancellationToken cancellationToken)
        {
            var manager = await _readManager.Execute(
                    new ManagerWithClientQuery(managerId, clientId),
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
