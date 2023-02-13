using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Data.Context.ModelsConfiguration
{
    internal class CompletedOrderConfiguration : IEntityTypeConfiguration<CompletedOrder>
    {
        public void Configure(EntityTypeBuilder<CompletedOrder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Created)
                .IsRequired()
                .HasColumnType("datetimeoffset(0)");
            builder.Property(x => x.Description)
                .IsRequired();
            builder.Property(x => x.Assigned)
                .IsRequired()
                .HasColumnType("datetimeoffset(0)");
            builder.Property(x => x.Closed)
                .IsRequired()
                .HasColumnType("datetimeoffset(0)");
            builder.Property(x => x.Status)
                .IsRequired();
            builder.Property(x => x.Comment)
                .IsRequired();
            builder.HasOne<Client>()
                .WithMany(c => c.CompletedOrders)
                .HasForeignKey(x => x.ClientId)
                .IsRequired();
            builder.HasOne<Manager>()
                .WithMany(m => m.CompletedOrders)
                .HasForeignKey(x => x.ManagerId)
                .IsRequired();
        }
    }
}
