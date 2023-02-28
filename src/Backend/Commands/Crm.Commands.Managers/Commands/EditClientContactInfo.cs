using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Commands.Core.Managers;
using Crm.Commands.Managers.Commands.Shared;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Managers.Commands
{
    public record EditClientContactInfoCommand(
    Guid ManagerId,
    Guid ClientId,
    string Email,
    string PhoneNumber) : IRequest<Result>;

    internal class EditClientContactInfoHandler : IRequestHandler<EditClientContactInfoCommand, Result>
    {
        private readonly IReadRepository<Manager> _readManager;
        private readonly IWriteRepository<Manager> _writeManager;

        public EditClientContactInfoHandler(IReadRepository<Manager> readManager, IWriteRepository<Manager> writeManager)
        {
            _readManager = readManager;
            _writeManager = writeManager;
        }

        public async Task<Result> Handle(EditClientContactInfoCommand request, CancellationToken cancellationToken)
        {
            var manager = await GetManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
            manager.SetClientContactInfo(request.ClientId, request.Email, request.PhoneNumber);
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
