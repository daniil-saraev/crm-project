using Microsoft.Extensions.DependencyInjection;
using Crm.Commands.Core.ExceptionHandler;
using MediatR;
using System.Reflection;

namespace Crm.Commands.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehaviorReturnResult<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehaviorReturnResultWithGuid<,>));
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
