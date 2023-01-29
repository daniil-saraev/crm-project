using Crm.Backend.Core.Models.Managers;
using Crm.Backend.Core.Models.Shared;

namespace Crm.Backend.Core.Models.Supervisors
{
    public class Supervisor : Entity
    {
        public FullName FullName { get; }
        public ContactInfo ContactInfo { get; }
        internal IList<Manager> Managers { get; init; }

        internal Supervisor(FullName fullName, ContactInfo contactInfo)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Managers = new List<Manager>();
        }
    }
}
