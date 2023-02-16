using Ardalis.GuardClauses;
using Ardalis.Result;
using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Supervisors;
using Crm.Shared.Repository;
using Crm.Supervisors.Queries;

namespace Crm.Supervisors.Commands
{
    public record AssignClientRequest(
        Guid SupervisorId,
        Guid ManagerId,
        Guid ClientId);

    public interface IAssignClient
    {
        Task<Result> Execute(AssignClientRequest request, CancellationToken cancellationToken);
    }

    internal class AssignClientHandler : IAssignClient
    {
        private readonly IWriteRepository<Supervisor> _writeSupervisor;
        private readonly IReadRepository<Supervisor> _readSupervisor;
        private readonly IReadRepository<Client> _readClient;

        public AssignClientHandler(
            IWriteRepository<Supervisor> writeSupervisor,
            IReadRepository<Supervisor> readSupervisor,
            IReadRepository<Client> readClient)
        {
            _writeSupervisor = writeSupervisor;
            _readSupervisor = readSupervisor;
            _readClient = readClient;
        }

        public async Task<Result> Execute(AssignClientRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var supervisor = await GetSupervisorWithManager(request.SupervisorId, request.ManagerId, cancellationToken);
                var client = await GetClientWithOrders(request.ClientId, cancellationToken);
                supervisor.AssignClient(request.ManagerId, client);
                return await SaveChangesAndReturnSuccess(supervisor, cancellationToken);
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

        private async Task<Supervisor> GetSupervisorWithManager(Guid supervisorId, Guid managerId, CancellationToken cancellationToken)
        {
            var supervisor = await _readSupervisor.Execute(
                new SupervisorWithManagerQuery(supervisorId, managerId),
                cancellationToken);
            if (supervisor == null)
                throw new NotFoundException(supervisorId.ToString(), nameof(Supervisor));
            return supervisor;
        }

        private async Task<Client> GetClientWithOrders(Guid id, CancellationToken cancellationToken)
        {
            var client = await _readClient.Execute(
                    new ClientWithOrdersQuery(id),
                    cancellationToken);
            if (client == null)
                throw new NotFoundException(id.ToString(), nameof(Client));
            return client;
        }

        private async Task<Result> SaveChangesAndReturnSuccess(Supervisor supervisor, CancellationToken cancellationToken)
        {
            await _writeSupervisor.Update(supervisor, cancellationToken);
            await _writeSupervisor.SaveChanges(cancellationToken);
            return Result.Success();
        }
    }
}
