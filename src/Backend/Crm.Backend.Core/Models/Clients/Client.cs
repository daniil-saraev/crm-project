using Ardalis.GuardClauses;
using Crm.Backend.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Backend.Core.Models.Clients
{
    public class Client : Entity
    {
        public string Name { get; private set; } = null!;
        public string Company { get; private set; } = null!;
        public ContactInfo ContactInfo{ get; }
        internal IList<Order> Orders { get; init; }

        internal Client(string name, ContactInfo contactInfo, string company)
        {
            SetName(name);
            SetCompany(company);
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Orders = new List<Order>();
        }

        internal void SetCompany(string company)
        {
            Company = Guard.Against.NullOrWhiteSpace(company, nameof(company));
        }

        internal void SetName(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
        }
    }
}
