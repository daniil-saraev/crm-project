using Ardalis.GuardClauses;
using Crm.Shared.Models;

namespace Crm.Core.Orders
{
    public abstract class Order : Entity
    {
        public Guid ClientId { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; private set; } = null!;

        protected Order() { }

        internal Order(Guid clientId, DateTimeOffset timeCreated, string description)
        {
            ClientId = Guard.Against.NullOrEmpty(clientId, nameof(clientId));
            if (timeCreated > DateTimeOffset.Now) throw new ArgumentException(nameof(timeCreated));
            Created = timeCreated;
            SetDescription(description);
        }

        internal void SetDescription(string description)
        {
            Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        }
    }
}
