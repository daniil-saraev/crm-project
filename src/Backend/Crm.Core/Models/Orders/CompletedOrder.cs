using Ardalis.GuardClauses;
using Crm.Core.Models.Managers;

namespace Crm.Core.Models.Orders
{
    public class CompletedOrder : Order
    {
        public Guid ManagerId { get; }
        public Manager Manager { get; } = null!;
        public DateTimeOffset Assigned { get; }
        public DateTimeOffset Closed { get; }
        public CompletionStatus Status { get; }
        public string Comment { get; }

        internal CompletedOrder(
            Guid clientId,
            Guid managerId,
            DateTimeOffset timeCreated,
            DateTimeOffset timeAssigned,
            string description,
            CompletionStatus completionStatus,
            string comment) : base(clientId, timeCreated, description)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
            Assigned = timeAssigned;
            Closed = DateTimeOffset.Now;
            Status = completionStatus;
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
