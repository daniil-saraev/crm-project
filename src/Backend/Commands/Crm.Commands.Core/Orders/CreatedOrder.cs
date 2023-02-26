namespace Crm.Commands.Core.Orders
{
    public class CreatedOrder : Order
    {
        private CreatedOrder() : base() { }
        internal CreatedOrder(Guid clientId, string description) : base(clientId, DateTimeOffset.Now, description)
        { }
    }
}
