using Ardalis.GuardClauses;
using Crm.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Models.Clients
{
    public class Client : Entity, IAggregateRoot
    {
        public Guid? ManagerId { get; private set; }
        public string Name { get; private set; } = null!;
        public ContactInfo ContactInfo { get; private set; } = null!;
        internal IList<CreatedOrder> CreatedOrders { get; init; }
        internal IList<OrderInWork> OrdersInWork { get; init; }
        internal IList<CompletedOrder> CompletedOrders { get; init; }

        internal Client(string name, ContactInfo contactInfo)
        {
            ManagerId = null;
            SetName(name);
            SetContactInfo(contactInfo);
            CreatedOrders = new List<CreatedOrder>();
            OrdersInWork = new List<OrderInWork>();
            CompletedOrders = new List<CompletedOrder>();
        }

        internal void SetName(string name)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        }

        internal void AssignManager(Guid managerId)
        {
            ManagerId = Guard.Against.NullOrEmpty(managerId, nameof(managerId));
        }

        internal void SetContactInfo(ContactInfo contactInfo)
        {
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        }
    }
}
