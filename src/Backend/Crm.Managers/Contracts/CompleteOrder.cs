using static Crm.Core.Models.Orders.CompletedOrder;

namespace Crm.Managers.Contracts;

public record CompleteOrder(
    Guid ManagerId,
    Guid OrderInWorkId,
    CompletionStatus Status,
    string Comment);
