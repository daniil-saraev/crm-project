using Crm.Core.Models;
using Crm.Core.Models.Managers;

namespace Crm.Core.Models.Supervisors
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
