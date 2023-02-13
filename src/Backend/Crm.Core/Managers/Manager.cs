using Ardalis.GuardClauses;
using Crm.Core.Clients;
using Crm.Core.Orders;
using Crm.Shared.Models;
using static Crm.Core.Orders.CompletedOrder;

namespace Crm.Core.Managers
{
    public class Manager : Entity, IAggregateRoot
    {
        private IList<OrderInWork> _ordersInWork = new List<OrderInWork>();
        private IList<CompletedOrder> _completedOrders = new List<CompletedOrder>();
        private IList<Client> _clients = new List<Client>();

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

        internal void AcceptOrder(CreatedOrder order)
        {
            var orderInWork = new OrderInWork(
                Id,
                order.ClientId,
                order.Created,
                order.Description);
            _ordersInWork.Add(orderInWork);
        }

        public void CompleteOrder(Guid orderInWorkId, CompletionStatus status, string comment)
        {
            var orderInWork = FindOrderInWork(orderInWorkId);
            var completedOrder = new CompletedOrder(
                orderInWork.ClientId,
                Id,
                orderInWork.Created,
                orderInWork.Assigned,
                orderInWork.Description,
                status,
                comment);

            _ordersInWork.Remove(orderInWork);
            _completedOrders.Add(completedOrder);
        }

        public void SetOrderDescription(Guid orderInWorkId, string description)
        {
            var order = FindOrderInWork(orderInWorkId);
            order.SetDescription(description);
        }

        private OrderInWork FindOrderInWork(Guid orderInWorkId)
        {
            Guard.Against.NullOrEmpty(orderInWorkId, nameof(orderInWorkId));
            var orderInWork = _ordersInWork.FirstOrDefault(order => order.Id == orderInWorkId);
            if (orderInWork == null)
                throw new NotFoundException(orderInWorkId.ToString(), nameof(OrderInWork));
            return orderInWork;
        }

        public void SetClientName(Guid clientId, string name)
        {
            var client = FindClient(clientId);
            client.SetName(name);
        }

        public void SetClientContactInfo(Guid clientId, string email, string phoneNumber)
        {
            var client = FindClient(clientId);
            client.SetContactInfo(
                new ContactInfo(email, phoneNumber));
        }

        private Client FindClient(Guid clientId)
        {
            Guard.Against.NullOrEmpty(clientId);
            var client = _clients.FirstOrDefault(client => client.Id == clientId);
            if (client == null)
                throw new NotFoundException(clientId.ToString(), nameof(Client));
            return client;
        }
    }
}
