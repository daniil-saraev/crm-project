using Ardalis.GuardClauses;

namespace Crm.Core.Models.Orders
{
    public class OrderInWork : Order
    {
        public Guid ManagerId { get; }
        public DateTimeOffset Assigned { get; }

        internal OrderInWork(Guid managerId,
            Guid clientId,
            DateTimeOffset timeCreated,
            string description) : base(clientId, timeCreated, description)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
            Assigned = DateTimeOffset.Now;
        }
    }
}
