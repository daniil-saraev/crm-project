using Ardalis.GuardClauses;
using Crm.Core.Models.Orders;
using Crm.Shared.Models;

namespace Crm.Core.Models.Clients
{
    public class Client : Entity
    {
        public FullName Name { get; private set; } = null!;
        public string? Company { get; private set; }
        public ContactInfo ContactInfo { get; }
        internal IList<Order> Orders { get; init; }

        internal Client(FullName name, ContactInfo contactInfo)
        {
            SetName(name);
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Orders = new List<Order>();
        }

        internal Client(FullName name, ContactInfo contactInfo, string company) : this(name, contactInfo)
        {
            SetCompany(company);
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
