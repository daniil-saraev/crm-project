using Crm.Messages.Bus.Configuration;
using Crm.Shared.Messages;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Messages.Bus
{
    public static class MessageBusModule
    {
        public static void LoadMessageBusModule(this IServiceCollection services, MessageConfiguration settings, params Assembly[] assemblies)
        {
            services.AddTransient<IMessageBus, MessageBus>();
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumers(assemblies);
                cfg.UsingRabbitMq((ctx, config) =>
                {
                    config.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });
                    config.UseMessageRetry(r => r.Interval(settings.RetryCount, TimeSpan.FromSeconds(settings.RetryIntervalSeconds)));
                    config.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds));
                    config.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}