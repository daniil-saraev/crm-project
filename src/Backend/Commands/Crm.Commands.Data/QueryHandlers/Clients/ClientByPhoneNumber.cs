using Crm.Commands.Clients.Commands;
using Crm.Commands.Core.Clients;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crm.Commands.Data.QueryHandlers.Clients
{
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
