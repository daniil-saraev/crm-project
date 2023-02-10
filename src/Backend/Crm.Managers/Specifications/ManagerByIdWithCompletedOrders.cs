using Ardalis.Specification;
using Crm.Core.Models.Managers;

namespace Crm.Managers.Specifications
{
    public class ManagerByIdWithCompletedOrders : Specification<Manager>, ISingleResultSpecification
    {
        public ManagerByIdWithCompletedOrders(Guid managerId)
        {
            Query.Where(manager => manager.Id == managerId).Include(manager => manager.CompletedOrders);
        }
    }
}
