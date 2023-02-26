using Crm.Commands.Core.Managers;
using Crm.Shared.Repository;

namespace Crm.Commands.Managers.Commands.Shared
{
    public record ManagerWithClientQuery(
        Guid ManagerId,
        Guid ClientId) : ISingleQuery<Manager>;
}
