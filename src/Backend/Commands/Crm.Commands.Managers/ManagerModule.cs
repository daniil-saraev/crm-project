using Crm.Commands.Core.ExceptionHandler;
using Crm.Messages.Bus;
using Crm.Messages.Bus.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Commands.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services, MessageConfiguration messageConfiguration)
        {
            services.LoadMessageBusModule(messageConfiguration);
            services.AddMediatR(config =>
            {
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<>));
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
