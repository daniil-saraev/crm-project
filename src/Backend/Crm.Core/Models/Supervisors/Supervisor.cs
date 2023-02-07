using Crm.Core.Models.Managers;

namespace Crm.Core.Models.Supervisors
{
    public class Supervisor : Entity, IAggregateRoot
    {
        internal IList<Manager> Managers { get; init; }

        internal Supervisor()
        {
            Managers = new List<Manager>();
        }
    }
}
