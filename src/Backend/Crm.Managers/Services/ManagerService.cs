using Ardalis.Result;
using Crm.Managers.Interfaces;
using Ardalis.GuardClauses;
using Crm.Core.Managers;
using Crm.Shared.Repository;
using Crm.Managers.Queries;

namespace Crm.Managers.Services
{
    internal class ManagerService : IManagerService
    {
        private readonly IWriteRepository<Manager> _writeRepository;
        private readonly IReadRepository<Manager> _readRepository;
        
        public ManagerService(IWriteRepository<Manager> writeRepository, IReadRepository<Manager> readRepository) 
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public async Task<Result> CompleteOrder(CompleteOrder request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var manager = await FindManagerWithOrder(request.ManagerId, request.OrderInWorkId, cancellationToken);
                manager.CompleteOrder(request.OrderInWorkId, request.Status, request.Comment);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
            });
        }

        public async Task<Result> EditOrderDescription(EditOrderDescription request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var manager = await FindManagerWithOrder(request.ManagerId, request.OrderInWorkId, cancellationToken);
                manager.SetOrderDescription(request.OrderInWorkId, request.Description);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
            });
        }

        public async Task<Result> EditClientContactInfo(EditClientInfo request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var manager = await FindManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
                manager.SetClientContactInfo(request.ClientId, request.Email, request.PhoneNumber);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
            });
        }

        public async Task<Result> EditClientName(EditClientName request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var manager = await FindManagerWithClient(request.ManagerId, request.ClientId, cancellationToken);
                manager.SetClientName(request.ClientId, request.Name);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
            });
        }

        private async Task<Manager> FindManagerWithOrder(Guid managerId, Guid orderInWorkId, CancellationToken cancellationToken)
        {
            var manager = await _readRepository.Execute(
            new ManagerWithOrderInWorkQuery(managerId, orderInWorkId),
                    cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }

        private async Task<Manager> FindManagerWithClient(Guid managerId, Guid clientId, CancellationToken cancellationToken)
        {
            var manager = await _readRepository.Execute(
                    new ManagerWithClientQuery(managerId, clientId),
                    cancellationToken);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }


        private async Task<Result> SaveChangesAndReturnSuccess(Manager manager, CancellationToken cancellationToken)
        {
            await _writeRepository.Update(manager, cancellationToken);
            await _writeRepository.SaveChanges(cancellationToken);
            return Result.Success();
        }

        private async Task<Result> ExecuteAndReturnResult(Func<Task<Result>> operation)
        {
            try
            {
                return await operation();
            }
            catch (NotFoundException ex)
            {
                return Result.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }
    }
}