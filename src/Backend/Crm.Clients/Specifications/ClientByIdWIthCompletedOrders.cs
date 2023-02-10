using Ardalis.Specification;
using Crm.Core.Models.Clients;
using Crm.Core.Models.Orders;

namespace Crm.Clients.Specifications
{
    public class ClientByIdWIthCompletedOrders : Specification<Client>, ISingleResultSpecification
    {
        public ClientByIdWIthCompletedOrders(Guid clientId) 
        {
            Query.Where(client => client.Id== clientId).Include(client => client.CompletedOrders);
        }
    }
}
