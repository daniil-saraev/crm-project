using Crm.Core.Clients;

namespace Crm.Core.Orders
{
    public class CreatedOrder : Order
    {
        internal CreatedOrder(Guid clientId, string description) : base(clientId, DateTimeOffset.Now, description)
        { }
    }
}
