using Crm.Clients.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Clients
{
    public static class ClientModule
    {
        public static void LoadClientModule(this IServiceCollection services)
        {
            services.AddTransient<ICreateOrder, CreateOrderHandler>();
            services.AddMediatR(config => config.AsTransient(), Assembly.GetExecutingAssembly());
        }
    }
}
