using Crm.Commands.Core.ExceptionHandler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Commands.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services)
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
