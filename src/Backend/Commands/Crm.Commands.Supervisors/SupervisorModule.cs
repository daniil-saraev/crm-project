using Microsoft.Extensions.DependencyInjection;
using Crm.Messages.Bus.Configuration;
using Crm.Messages.Bus;

namespace Crm.Commands.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services, MessageConfiguration messageConfiguration)
        {
            services.LoadMessageBusModule(messageConfiguration);
        }
    }
}
