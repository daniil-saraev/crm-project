using Ardalis.GuardClauses;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Supervisors
{
    public class Supervisor : Entity, IAggregateRoot
    {
        private IList<Manager> _managers = new List<Manager>();

        public IReadOnlyCollection<Manager> Managers => _managers.AsReadOnly();

        internal Supervisor()
        {
        }

        public void AddNewManager(Guid managerAccountId)
        {
            Guard.Against.NullOrEmpty(managerAccountId);
            if (_managers.FirstOrDefault(manager => manager.Id == managerAccountId) != null)
                throw new InvalidOperationException();

            var manager = new Manager(Id)
            {
                Id = managerAccountId
            };
            _managers.Add(manager);
        }

        private void AcceptManager(Manager manager)
        {
            manager.SetSupervisor(Id);
            _managers.Add(manager);
        }

        public void TransferManager(Guid managerId, Supervisor newSupervisor)
        {
            var manager = FindManager(managerId);
            _managers.Remove(manager);
            newSupervisor.AcceptManager(manager);
        }


        public void AssignOrder(Guid managerId, CreatedOrder createdOrder)
        {
            var manager = FindManager(managerId);
            manager.AcceptOrder(createdOrder);
        }

        private Manager FindManager(Guid managerId)
        {
            Guard.Against.NullOrEmpty(managerId);
            var manager = _managers.FirstOrDefault(manager => manager.Id == managerId);
            if (manager == null)
                throw new NotFoundException(managerId.ToString(), nameof(Manager));
            return manager;
        }
    }
}
