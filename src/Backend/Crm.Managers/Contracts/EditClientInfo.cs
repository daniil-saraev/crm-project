namespace Crm.Managers.Contracts;

public record EditClientInfo(
    Guid ManagerId,
    Guid ClientId,
    string Email,
    string PhoneNumber);