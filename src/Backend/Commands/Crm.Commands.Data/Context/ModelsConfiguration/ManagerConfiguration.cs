using Crm.Commands.Core.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Commands.Data.Context.ModelsConfiguration
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
