using Ardalis.GuardClauses;

namespace Crm.Core.Models.Orders
{
    public abstract class Order : Entity
    {
        public Guid ClientId { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; private set; }

        internal Order(Guid clientId, DateTimeOffset timeCreated)
        {
            ClientId = Guard.Against.NullOrEmpty(clientId, nameof(clientId));
            if (timeCreated > DateTimeOffset.Now) throw new ArgumentException(nameof(timeCreated));
            Created = timeCreated;
            Description = string.Empty;
        }

        internal Order(Guid clientId, DateTimeOffset timeCreated, string description) : this(clientId, timeCreated)
        {
            SetDescription(description);
        }

        internal void SetDescription(string description)
        {
            Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        }
    }
}
