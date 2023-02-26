using Microsoft.Extensions.DependencyInjection;
using Crm.Messages.Bus.Configuration;
using Crm.Messages.Bus;

namespace Crm.Commands.Clients
{
    public static class ClientModule
    {
        public static void LoadClientModule(this IServiceCollection services, MessageConfiguration messageConfiguration)
        {
            services.LoadMessageBusModule(messageConfiguration);
        }
    }
}
