using Crm.Backend.Core.Models.Managers;

namespace Crm.Backend.Core.Models.Supervisors
{
    public class Supervisor : Entity
    {
        internal IList<Manager> Managers { get; init; }

        internal Supervisor()
        {
            Managers = new List<Manager>();
        }
    }
}
