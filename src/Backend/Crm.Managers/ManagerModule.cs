using Crm.Managers.Interfaces;
using Crm.Managers.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services)
        {
            services.AddScoped<IManagerService, ManagerService>();
            services.AddMediatR(config => config.AsScoped(), Assembly.GetExecutingAssembly());
        }
    }
}
