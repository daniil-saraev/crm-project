using Ardalis.GuardClauses;

namespace Crm.Commands.Core.Orders
{
    public class OrderInWork : Order
    {
        public Guid ManagerId { get; private set; }
        public DateTimeOffset Assigned { get; }

        private OrderInWork() : base() { }

        internal OrderInWork(
            Guid managerId,
            Guid clientId,
            DateTimeOffset timeCreated,
            string description) : base(clientId, timeCreated, description)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
            Assigned = DateTimeOffset.Now;
        }

        internal void AssignManager(Guid managerId)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId);
        }
    }
}
