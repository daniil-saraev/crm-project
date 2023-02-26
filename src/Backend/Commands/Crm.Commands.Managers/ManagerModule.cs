using Crm.Messages.Bus;
using Crm.Messages.Bus.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Commands.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services, MessageConfiguration messageConfiguration)
        {
            services.LoadMessageBusModule(messageConfiguration);
        }
    }
}
