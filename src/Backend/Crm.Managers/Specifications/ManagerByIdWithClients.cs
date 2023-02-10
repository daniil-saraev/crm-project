using Ardalis.Specification;
using Crm.Core.Models.Managers;

namespace Crm.Managers.Specifications
{
    public class ManagerByIdWithClients : Specification<Manager>, ISingleResultSpecification
    {
        public ManagerByIdWithClients(Guid managerId)
        {
            Query.Where(manager => manager.Id == managerId).Include(manager => manager.Clients);
        }
    }
}
