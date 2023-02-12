using Crm.Clients.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Clients
{
    public static class ClientModule
    {
        public static void LoadClientModule(this IServiceCollection services)
        {
            services.AddScoped<ClientService, ClientService>();
            services.AddMediatR(config => config.AsScoped(), Assembly.GetExecutingAssembly());
        }
    }
}
