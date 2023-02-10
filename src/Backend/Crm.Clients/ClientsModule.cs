using Crm.Clients.Interfaces;
using Crm.Clients.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Clients
{
    public static class ClientsModule
    {
        public static void LoadClientsModule(this IServiceCollection services)
        {
            services.AddSingleton<ICreateOrderService, CreateOrderService>();
        }
    }
}
