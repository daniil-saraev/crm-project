using Ardalis.Specification;
using Crm.Core.Models.Managers;

namespace Crm.Managers.Specifications
{
    public class ManagerByIdWIthOrdersInWork : Specification<Manager>, ISingleResultSpecification
    {
        public ManagerByIdWIthOrdersInWork(Guid managerId)
        {
            Query.Where(manager => manager.Id == managerId).Include(manager => manager.OrdersInWork);
        }
    }
}
