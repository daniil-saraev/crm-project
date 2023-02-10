namespace Crm.Managers.Contracts;

public record EditClientName(
    Guid ManagerId, 
    Guid ClientId,
    string Name);