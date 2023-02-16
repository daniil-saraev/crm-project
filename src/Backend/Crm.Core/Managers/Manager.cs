using Ardalis.GuardClauses;
using Crm.Core.Clients;
using Crm.Core.Orders;
using Crm.Shared.Models;
using static Crm.Core.Orders.CompletedOrder;

namespace Crm.Core.Managers
{
    public class Manager : Entity, IAggregateRoot
    {
        private List<OrderInWork> _ordersInWork = new List<OrderInWork>();
        private List<CompletedOrder> _completedOrders = new List<CompletedOrder>();
        private List<Client> _clients = new List<Client>();

        public Guid SupervisorId { get; private set; }
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
        public IReadOnlyCollection<Client> Clients
        {
            get { return _clients.AsReadOnly(); }
            private set { _clients = value.ToList(); }
        }

        private Manager() { }

        public Manager(Guid supervisorId)
        {
            SetSupervisor(supervisorId);
        }

        internal void SetSupervisor(Guid supervisorId)
        {
            SupervisorId = Guard.Against.NullOrEmpty(supervisorId, nameof(supervisorId));
        }

        internal void AcceptClient(Client client)
        {
            _clients.Add(client);
            _ordersInWork.AddRange(client.OrdersInWork.Select(order =>
            {
                order.AssignManager(Id);
                return order;
            }));
            _ordersInWork.AddRange(client.CreatedOrders.Select(order =>
            {
                return new OrderInWork(
                Id,
                order.ClientId,
                order.Created,
                order.Description);
            }));
        }

        internal Client GiveClient(Guid clientId)
        {
            var client = FindClient(clientId);
            _clients.Remove(client);
            return client;
        }

        public void TakeOrder(Guid createdOrderId, Guid clientId)
        {
            var client = FindClient(clientId);
            var order = client.TakeOrderToWork(createdOrderId);
            _ordersInWork.Add(new OrderInWork(
                this.Id,
                order.ClientId,
                order.Created,
                order.Description));
        }

        public void CompleteOrder(Guid orderInWorkId, CompletionStatus status, string comment)
        {
            var orderInWork = FindOrderInWork(orderInWorkId);
            var client = FindClient(orderInWork.ClientId);
            var completedOrder = new CompletedOrder(
                orderInWork.ClientId,
                Id,
                orderInWork.Created,
                orderInWork.Assigned,
                orderInWork.Description,
                status,
                comment);
            _completedOrders.Add(completedOrder);
            _ordersInWork.Remove(orderInWork);
            client.CompleteOrder(orderInWorkId);
        }

        public void SetOrderDescription(Guid orderInWorkId, string description)
        {
            var orderInWork = FindOrderInWork(orderInWorkId);
            orderInWork.SetDescription(description);
        }

        public void SetClientName(Guid clientId, string name)
        {
            var client = FindClient(clientId);
            client.SetName(name);
        }

        public void SetClientContactInfo(Guid clientId, string email, string phoneNumber)
        {
            var client = FindClient(clientId);
            client.SetContactInfo(new ContactInfo(email, phoneNumber));
        }

        private Client FindClient(Guid clientId)
        {
            Guard.Against.NullOrEmpty(clientId);
            var client = _clients.FirstOrDefault(client => client.Id == clientId);
            if (client == null)
                throw new NotFoundException(clientId.ToString(), nameof(Client));
            return client;
        }

        private OrderInWork FindOrderInWork(Guid orderId)
        {
            Guard.Against.NullOrEmpty(orderId);
            var order = _ordersInWork.FirstOrDefault(order => order.Id == orderId);
            if (order == null) 
                throw new NotFoundException(orderId.ToString(), nameof(OrderInWork));
            return order;
        }
    }
}
