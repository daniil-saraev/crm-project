using Ardalis.GuardClauses;
using Crm.Core.Managers;

namespace Crm.Core.Orders
{
    public class OrderInWork : Order
    {
        public Guid ManagerId { get; }
        public Manager Manager { get; } = null!;
        public DateTimeOffset Assigned { get; }

        internal OrderInWork(
            Guid managerId,
            Guid clientId,
            DateTimeOffset timeCreated,
            string description) : base(clientId, timeCreated, description)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
            Assigned = DateTimeOffset.Now;
        }
    }
}
