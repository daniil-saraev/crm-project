using Ardalis.GuardClauses;
using Crm.Core.Models.Clients;
using Crm.Core.Models.Orders;

namespace Crm.Core.Models.Managers
{
    public class Manager : Entity
    {
        public Guid SupervisorId { get; private set; }
        internal IList<OrderInWork> OrdersInWork { get; init; }
        internal IList<CompletedOrder> CompletedOrders { get; init; }
        internal IList<Client> Clients { get; init; }

        internal Manager(Guid supervisorId)
        {
            SetSupervisor(supervisorId);
            OrdersInWork = new List<OrderInWork>();
            CompletedOrders = new List<CompletedOrder>();
            Clients = new List<Client>();
        }

        internal void SetSupervisor(Guid supervisorId)
        {
            SupervisorId = Guard.Against.NullOrEmpty(supervisorId, nameof(supervisorId));
        }
    }
}
