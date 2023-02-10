using Ardalis.Specification;
using Crm.Core.Models.Clients;

namespace Crm.Clients.Specifications
{
    public class ClientByIdWithOrdersInWork : Specification<Client>, ISingleResultSpecification
    {
        public ClientByIdWithOrdersInWork(Guid clientId)
        {
            Query.Where(client => client.Id== clientId).Include(client => client.OrdersInWork);
        }
    }
}
