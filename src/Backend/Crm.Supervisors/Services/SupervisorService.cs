using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Orders;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Interfaces;
using Crm.Supervisors.Queries;

namespace Crm.Supervisors.Services
{
    internal class SupervisorService : ISupervisorService
    {
        private readonly IWriteRepository<Supervisor> _writeRepository;
        private readonly IReadRepository<Supervisor> _supervisorRepository;
        private readonly IReadRepository<CreatedOrder> _ordersRepository;

        public SupervisorService(
            IWriteRepository<Supervisor> writeRepository, 
            IReadRepository<Supervisor> supervisorRepository,
            IReadRepository<CreatedOrder> ordersRepository)
        {
            _writeRepository = writeRepository;
            _supervisorRepository = supervisorRepository;
            _ordersRepository = ordersRepository;
        }

        public async Task<Result> AddNewManager(AddNewManager request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var supervisor = await FindSupervisorById(request.SupervisorId, cancellationToken);
                supervisor.AddNewManager(request.ManagerAccountId);
                return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
            });
        }

        public async Task<Result> AssignOrder(AssignOrder request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var supervisor = await FindSupervisorWithManager(request.SupervisorId, request.ManagerId, cancellationToken);
                var order = await FindOrderById(request.CreatedOrderId, cancellationToken);
                supervisor.AssignOrder(request.ManagerId, order);
                return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
            });
        }

        public async Task<Result> TransferManager(TransferManager request, CancellationToken cancellationToken)
        {
            return await ExecuteAndReturnResult(async () =>
            {
                var supervisor = await FindSupervisorWithManager(request.SupervisorId, request.ManagerId, cancellationToken);
                var toSupervisor = await FindSupervisorById(request.ToSupervisorId, cancellationToken);
                supervisor.TransferManager(request.ManagerId, toSupervisor);
                await _writeRepository.Update(toSupervisor, cancellationToken);
                return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
            });
        }

        private async Task<Supervisor> FindSupervisorById(Guid id, CancellationToken cancellationToken)
        {
            var supervisor = await _supervisorRepository.Execute(
                new SupervisorByIdQuery(id),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(id.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<Supervisor> FindSupervisorWithManager(Guid supervisorId, Guid managerId, CancellationToken cancellationToken)
        {
            var supervisor = await _supervisorRepository.Execute(
                new SupervisorWithManagerQuery(supervisorId, managerId),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(supervisorId.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<CreatedOrder> FindOrderById(Guid id, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.Execute(
                    new CreatedOrderByIdQuery(id),
                    cancellationToken);
            if (order == null)
                throw new NotFoundException(id.ToString(), nameof(CreatedOrder));
            return order;
        }

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeRepository.Update(supervisor, cancellationToken);
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
