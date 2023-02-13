using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Core.Supervisors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Data.Context.ModelsConfiguration
{
    internal class ManagerConfiguration : IEntityTypeConfiguration<Manager>
    {
        public void Configure(EntityTypeBuilder<Manager> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.CompletedOrders)
                .WithOne()
                .HasForeignKey(o => o.ManagerId)
                .IsRequired();
        }
    }
}
