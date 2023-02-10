namespace Crm.Managers.Contracts;

public record EditOrderDescription(
    Guid ManagerId,
    Guid OrderInWorkId,
    string Description);
