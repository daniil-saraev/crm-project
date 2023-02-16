using Crm.Managers.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Managers
{
    public static class ManagerModule
    {
        public static void LoadManagerModule(this IServiceCollection services)
        {
            services.AddTransient<ICompleteOrder, CompleteOrderHandler>();
            services.AddTransient<IEditOrderDescription, EditOrderDescriptionHandler>();
            services.AddTransient<IEditClientContactInfo, EditClientContactInfoHandler>();
            services.AddTransient<IEditClientName, EditClientNameHandler>();
            services.AddMediatR(config => config.AsScoped(), Assembly.GetExecutingAssembly());
        }
    }
}
