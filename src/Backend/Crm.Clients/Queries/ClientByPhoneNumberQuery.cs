using Crm.Core.Clients;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Clients.Queries
{
    internal record ClientByPhoneNumberQuery(
        string PhoneNumber) : ISingleQuery<Client>;

    internal class ClientByPhoneNumberHandler : ISingleQueryHandler<ClientByPhoneNumberQuery, Client>
    {
        private readonly DbContext _dataContext;

        public ClientByPhoneNumberHandler(DbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Client?> Handle(ClientByPhoneNumberQuery request, CancellationToken cancellationToken)
        {
            return await _dataContext.Set<Client>().FirstOrDefaultAsync(
                client => client.ContactInfo.PhoneNumber == request.PhoneNumber,
                cancellationToken);
        }
    }
}
