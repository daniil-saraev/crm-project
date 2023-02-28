using Microsoft.Extensions.DependencyInjection;
using Crm.Messages.Bus.Configuration;
using Crm.Messages.Bus;
using Crm.Commands.Core.ExceptionHandler;
using MediatR;
using System.Reflection;

namespace Crm.Commands.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services, MessageConfiguration messageConfiguration)
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
