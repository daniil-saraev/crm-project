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
            services.AddTransient<IEventBus, EventBus>();
            services.AddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddMediatR(config => config.AsTransient(), Assembly.GetExecutingAssembly());   
        }
    }
}
