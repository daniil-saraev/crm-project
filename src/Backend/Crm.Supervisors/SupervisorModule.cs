using Crm.Shared.ExceptionHandler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>));
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
