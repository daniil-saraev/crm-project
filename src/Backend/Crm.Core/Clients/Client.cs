using Ardalis.GuardClauses;
using Crm.Core.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Clients
{
    public class Client : Entity, IAggregateRoot
    {
        private IList<CreatedOrder> _createdOrders = new List<CreatedOrder>();
        private IList<OrderInWork> _ordersInWork = new List<OrderInWork>();
        private IList<CompletedOrder> _completedOrders = new List<CompletedOrder>();

        public Guid? ManagerId { get; private set; }
        public string Name { get; private set; } = null!;
        public ContactInfo ContactInfo { get; private set; } = null!;
        public IReadOnlyCollection<CreatedOrder> CreatedOrders
        {
            get { return _createdOrders.AsReadOnly(); }
            private set { _createdOrders = value.ToList(); }
        }
        public IReadOnlyCollection<OrderInWork> OrdersInWork
        {
            get { return _ordersInWork.AsReadOnly(); }
            private set { _ordersInWork = value.ToList(); }
        }
        public IReadOnlyCollection<CompletedOrder> CompletedOrders
        {
            get { return _completedOrders.AsReadOnly(); }
            private set { _completedOrders = value.ToList(); }
        }

        private Client() { }

        public Client(string name, ContactInfo contactInfo)
        {
            ManagerId = null;
            SetName(name);
            SetContactInfo(contactInfo);
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
            ContactInfo = Guard.Against.Null(contactInfo, nameof(contactInfo));
        }

        public void PlaceOrder(string description)
        {
            var order = new CreatedOrder(
                Id,
                Guard.Against.NullOrWhiteSpace(description, nameof(description)));
            _createdOrders.Add(order);
        }
    }
}
