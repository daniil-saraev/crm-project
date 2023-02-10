using Ardalis.Specification;
using Crm.Core.Models.Clients;

namespace Crm.Clients.Specifications
{
    public class ClientByPhoneNumber : Specification<Client>, ISingleResultSpecification
    {
        public ClientByPhoneNumber(string phoneNumber)
        {
            Query.Where(client => client.ContactInfo.PhoneNumber == phoneNumber);
        }
    }
}
