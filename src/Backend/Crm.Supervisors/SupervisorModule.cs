using Crm.Supervisors.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Supervisors
{
    public static class SupervisorModule
    {
        public static void LoadSupervisorModule(this IServiceCollection services)
        {
            services.AddTransient<IAddNewManager, AddNewManagerHandler>();
            services.AddTransient<IAssignClient, AssignClientHandler>();
            services.AddTransient<ITransferManager, TransferManagerHandler>();
            services.AddTransient<ITransferClient, TransferClientHandler>();
            services.AddMediatR(config => config.AsTransient(), Assembly.GetExecutingAssembly());
        }
    }
}
