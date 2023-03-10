using Crm.Commands.Data.Context;
using Crm.Commands.Data.Repository;
using Crm.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Crm.Commands.Data
{
    public static class DataModule
    {
        /// <summary>
        /// Registers DbContext and all associated services. If <paramref name="connectionString"/>
        /// is null, sets in-memory database.
        /// </summary>
        public static void LoadDataModule(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<DbContext, DataContext>(options =>
            {
                if (connectionString == null)
                    options.UseInMemoryDatabase(nameof(DataContext));
                else
                    options.UseSqlServer(connectionString);
            });

            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }

        /// <summary>
        /// Calls EnsureDeleted() then Migrate().
        /// </summary>
        public static void UseCleanDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;
                var dbContext = scopedProvider.GetRequiredService<DataContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Calls Migrate() only.
        /// </summary>
        public static void UseExistingDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;
                var dbContext = scopedProvider.GetRequiredService<DataContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
