using Crm.Shared.ExceptionHandler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>));
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
