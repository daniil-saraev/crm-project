using Crm.Core.Models.Clients;

namespace Crm.Core.Models.Orders
{
    public class CreatedOrder : Order
    {
        internal CreatedOrder(Guid clientId, string description) : base(clientId, DateTimeOffset.Now, description)
        { }
    }
}
