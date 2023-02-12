using Crm.Supervisors.Interfaces;
using Crm.Supervisors.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services)
        {
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddMediatR(config => config.AsScoped(), Assembly.GetExecutingAssembly());
        }
    }
}
