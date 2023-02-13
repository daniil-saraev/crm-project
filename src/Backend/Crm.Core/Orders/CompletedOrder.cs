using Ardalis.GuardClauses;

namespace Crm.Core.Orders
{
    public class CompletedOrder : Order
    {
        public Guid ManagerId { get; }
        public DateTimeOffset Assigned { get; }
        public DateTimeOffset Closed { get; }
        public CompletionStatus Status { get; }
        public string Comment { get; } = null!;

        private CompletedOrder() : base() { }

        internal CompletedOrder(
            Guid clientId,
            Guid managerId,
            DateTimeOffset created,
            DateTimeOffset assigned,
            string description,
            CompletionStatus status,
            string comment) : base(clientId, created, description)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
            Assigned = assigned;
            Closed = DateTimeOffset.Now;
            Status = status;
            Comment = Guard.Against.NullOrWhiteSpace(comment, nameof(comment));
        }

        public enum CompletionStatus
        {
            Fulfilled,
            Canceled,
            Rejected
        }
    }
}
