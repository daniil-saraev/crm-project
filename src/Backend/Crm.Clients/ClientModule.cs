using Crm.Shared.ExceptionHandler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Clients
{
    public static class ClientModule
    {
        public static void LoadClientModule(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>));
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
