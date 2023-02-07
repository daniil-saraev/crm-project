using Ardalis.Specification;
using Crm.Core.Models.Clients;

namespace Crm.Clients.Specifications
{
    public class GetByPhoneNumber : Specification<Client>
    {
        public GetByPhoneNumber(string phoneNumber)
        {
            Query.Where(client => client.ContactInfo.PhoneNumber == phoneNumber);
        }
    }
}
