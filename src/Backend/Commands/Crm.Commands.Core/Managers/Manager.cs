using Ardalis.GuardClauses;
using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Orders;
using Crm.Shared.Models;
using System.Collections.Immutable;
using static Crm.Commands.Core.Orders.CompletedOrder;

namespace Crm.Commands.Core.Managers
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

        internal static Manager New(Guid managerAccountId, Guid supervisorId)
        {
            return new Manager(supervisorId) { Id = managerAccountId };
        }

        private Manager(Guid supervisorId)
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
            var createdOrders = client.CreatedOrders.Select(o => o.Id).ToImmutableArray();
            foreach (var order in createdOrders)
                TakeOrder(order, client.Id);
        }

        internal Client GiveClient(Guid clientId)
        {
            var client = FindClient(clientId);
            _ordersInWork.RemoveAll(o => o.ClientId == clientId);
            _clients.Remove(client);
            return client;
        }

        public OrderInWork TakeOrder(Guid createdOrderId, Guid clientId)
        {
            var client = FindClient(clientId);
            var createdOrder = client.TakeOrderToWork(createdOrderId);
            var orderInWork = new OrderInWork(
                this.Id,
                createdOrder.ClientId,
                createdOrder.Created,
                createdOrder.Description);
            _ordersInWork.Add(orderInWork);
            client.AddOrderInWork(orderInWork);
            return orderInWork;
        }

        public CompletedOrder CompleteOrder(Guid orderInWorkId, Guid clientId, CompletionStatus status, string comment)
        {
            var orderInWork = FindOrderInWork(orderInWorkId);
            var client = FindClient(clientId);
            var completedOrder = new CompletedOrder(
                orderInWork.ClientId,
                Id,
                orderInWork.Created,
                orderInWork.Assigned,
                orderInWork.Description,
                status,
                comment);
            client.CompleteOrder(orderInWorkId, completedOrder);
            _ordersInWork.Remove(orderInWork);
            _completedOrders.Add(completedOrder);
            return completedOrder;
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