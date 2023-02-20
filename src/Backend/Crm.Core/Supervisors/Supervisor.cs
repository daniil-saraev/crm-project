using Ardalis.GuardClauses;
using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Shared.Models;

namespace Crm.Core.Supervisors
{
    public class Supervisor : Entity, IAggregateRoot
    {
        private IList<Manager> _managers = new List<Manager>();

        public IReadOnlyCollection<Manager> Managers
        {
            get { return _managers.AsReadOnly(); }
            set { _managers = value.ToList(); }
        }

        private Supervisor() { }

        public static Supervisor New(Guid supervisorAccountId)
        {
            return new Supervisor() { Id = supervisorAccountId };
        }

        public void AddNewManager(Guid managerAccountId)
        {
            Guard.Against.NullOrEmpty(managerAccountId);
            if (_managers.FirstOrDefault(manager => manager.Id == managerAccountId) != null)
                throw new InvalidOperationException();

            var manager = Manager.New(managerAccountId, Id);
            _managers.Add(manager);
        }

        private void AcceptManager(Manager manager)
        {
            manager.SetSupervisor(Id);
            _managers.Add(manager);
        }

        public void TransferManager(Guid managerId, Supervisor toSupervisor)
        {
            var manager = FindManager(managerId);
            _managers.Remove(manager);
            toSupervisor.AcceptManager(manager);
        }

        public void AssignClient(Guid managerId, Client client)
        {
            var manager = FindManager(managerId);
            if (client.ManagerId != null)
                throw new InvalidOperationException();
            client.AssignManager(manager.Id);
            manager.AcceptClient(client);
        }

        public void TransferClient(Guid fromManagerId, Guid toManagerId, Guid clientId)
        {
            var fromManager = FindManager(fromManagerId);
            var toManager = FindManager(toManagerId);
            var client = fromManager.GiveClient(clientId);
            client.AssignManager(toManager.Id);
            toManager.AcceptClient(client);
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
