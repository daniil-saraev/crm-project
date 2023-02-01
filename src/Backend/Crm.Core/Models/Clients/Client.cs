using Ardalis.GuardClauses;
using Crm.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Models.Clients
{
    public class Client : Entity
    {
        public FullName Name { get; private set; } = null!;
        public string Company { get; private set; } = null!;
        public ContactInfo ContactInfo { get; }
        internal IList<Order> Orders { get; init; }

        internal Client(FullName name, ContactInfo contactInfo, string company)
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

        internal void SetName(FullName name)
        {
            Name = name;
        }
    }
}
