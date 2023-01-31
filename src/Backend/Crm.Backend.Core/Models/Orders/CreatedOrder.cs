namespace Crm.Backend.Core.Models.Orders
{
    public class CreatedOrder : Order
    {
        internal CreatedOrder(Guid clientId) : base(clientId, DateTimeOffset.Now)
        { }

        internal CreatedOrder(Guid clientId, string description) : base(clientId, DateTimeOffset.Now, description)
        { }
    }
}
