using Crm.Shared.Messages;

namespace Crm.Messages.Clients
{
    public record CreateOrderCommand(
        string Name,
        string Email,
        string PhoneNumber,
        string Description) : ICommand;
}
