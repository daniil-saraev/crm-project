using Ardalis.Specification;
using Crm.Core.Models.Clients;
using Crm.Core.Models.Managers;

namespace Crm.Managers.Specifications
{
    public class ManagerClientById : Specification<Manager, Client?>, ISingleResultSpecification
    {
        public ManagerClientById(Guid managerId, Guid clientId)
        {
            Query.Where(manager => manager.Id == managerId)
                    .Include(manager => manager.Clients);
                        
            Query.Select(manager => manager.Clients.FirstOrDefault(client => client.Id == clientId));
        }
    }
}
