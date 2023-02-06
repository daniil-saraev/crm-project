using Ardalis.GuardClauses;
using Crm.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Models.Clients
{
    public class Client : Entity
    {
        public string Name { get; private set; } = null!;
        public ContactInfo ContactInfo { get; }
        internal IList<Order> Orders { get; init; }

        internal Client(string name, ContactInfo contactInfo)
        {
            SetName(name);
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Orders = new List<Order>();
        }

        internal void SetName(string name)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        }
    }
}
