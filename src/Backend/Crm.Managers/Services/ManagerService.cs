using Ardalis.Result;
using Crm.Managers.Contracts;
using Crm.Core.Interfaces;
using Crm.Managers.Interfaces;
using Crm.Core.Models.Managers;
using Ardalis.GuardClauses;
using Crm.Data.Context;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Crm.Managers.Services
{
    internal class ManagerService : IManagerService
    {
        private readonly IRepository<Manager> _repository;
        private readonly DataContext _dataContext;
        
        public ManagerService(IRepository<Manager> repository, DataContext dataContext) 
        {
            _repository = repository;
            _dataContext = dataContext;
        }

        public async Task<Result> CompleteOrder(CompleteOrder request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await _dataContext.Managers
                    .Where(manager => manager.Id == request.ManagerId)
                    .IncludeFilter(manager => manager.OrdersInWork
                    .Where(order => order.Id == request.OrderInWorkId)
                    .SingleOrDefault())
                    .SingleOrDefaultAsync(cancellationToken);

                if (manager == null)
                    return Result.NotFound(request.ManagerId.ToString(), nameof(Manager));

                manager.CompleteOrder(request.OrderInWorkId, request.Status, request.Comment);

                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
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

        public async Task<Result> EditOrderDescription(EditOrderDescription request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await _repository.GetByIdAsync(request.ManagerId, cancellationToken);
                if (manager == null)
                    return Result.NotFound(request.ManagerId.ToString(), nameof(Manager));

                manager.SetOrderDescription(request.OrderInWorkId, request.Description);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
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

        public async Task<Result> EditClientContactInfo(EditClientInfo request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await _repository.GetByIdAsync(request.ManagerId, cancellationToken);
                if (manager == null)
                    return Result.NotFound(request.ManagerId.ToString(), nameof(Manager));
                manager.SetClientContactInfo(request.ClientId, request.Email, request.PhoneNumber);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
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

        public async Task<Result> EditClientName(EditClientName request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await _repository.GetByIdAsync(request.ManagerId, cancellationToken);
                if (manager == null)
                    return Result.NotFound(request.ManagerId.ToString(), nameof(Manager));
                manager.SetClientName(request.ClientId, request.Name);
                return await SaveChangesAndReturnSuccess(manager, cancellationToken);
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

        private async Task<Result> SaveChangesAndReturnSuccess(Manager manager, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(manager, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
