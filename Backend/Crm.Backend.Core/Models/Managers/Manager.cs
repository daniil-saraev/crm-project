using Ardalis.GuardClauses;
using Crm.Backend.Core.Models.Orders;
using Crm.Backend.Core.Models.Shared;

namespace Crm.Backend.Core.Models.Managers
{
    public class Manager : Entity
    {
        public Guid SupervisorId { get; private set; }
        public FullName FullName { get; }
        public ContactInfo ContactInfo { get; }
        internal IList<OrderInWork> OrdersInWork { get; init; }
        internal IList<CompletedOrder> CompletedOrders { get; init; }

        internal Manager(FullName fullName, ContactInfo contactInfo, Guid supervisorId)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            SetSupervisor(supervisorId);
            OrdersInWork = new List<OrderInWork>();
            CompletedOrders = new List<CompletedOrder>();
        }

        internal void SetSupervisor(Guid supervisorId)
        {
            SupervisorId = Guard.Against.NullOrEmpty(supervisorId, nameof(supervisorId));
        }
    }
}
