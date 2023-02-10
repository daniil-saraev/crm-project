using Ardalis.GuardClauses;
using Crm.Core.Models.Managers;
using Crm.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Models.Clients
{
    public class Client : Entity, IAggregateRoot
    {
        private IList<CreatedOrder> _createdOrders = new List<CreatedOrder>();
        private IList<OrderInWork> _ordersInWork = new List<OrderInWork>();
        private IList<CompletedOrder> _completedOrders = new List<CompletedOrder>();

        public Manager? Manager { get; private set; }
        public string Name { get; private set; } = null!;
        public ContactInfo ContactInfo { get; private set; } = null!;
        public IReadOnlyCollection<CreatedOrder> CreatedOrders => _createdOrders.AsReadOnly();
        public IReadOnlyCollection<OrderInWork> OrdersInWork => _ordersInWork.AsReadOnly();
        public IReadOnlyCollection<CompletedOrder> CompletedOrders => _completedOrders.AsReadOnly();

        private Client() { }

        public Client(string name, ContactInfo contactInfo)
        {
            Manager = null;
            SetName(name);
            SetContactInfo(contactInfo);
        }

        internal void SetName(string name)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        }

        internal void AssignManager(Manager manager)
        {
            Manager = Guard.Against.Null(manager, nameof(manager));
        }

        internal void SetContactInfo(ContactInfo contactInfo)
        {
            ContactInfo = Guard.Against.Null(contactInfo, nameof(contactInfo)); 
        }

        public void PlaceOrder(string description)
        {
            var order = new CreatedOrder(
                this.Id,
                Guard.Against.NullOrWhiteSpace(description, nameof(description)));
            _createdOrders.Add(order);
        }
    }
}
