namespace Crm.Clients.Contracts
{
    public record CreateOrder(
        string Name, 
        string Email, 
        string PhoneNumber, 
        string Description);
}
