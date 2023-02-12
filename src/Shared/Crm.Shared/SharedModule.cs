using Crm.Shared.Events;
using Crm.Shared.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Shared
{
    public static class SharedModule
    {
        public static void LoadSharedModule(this IServiceCollection services)
        {
            services.AddScoped<IEventBus, EventBus>();
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddMediatR(config => config.AsScoped(), Assembly.GetExecutingAssembly());   
        }
    }
}
