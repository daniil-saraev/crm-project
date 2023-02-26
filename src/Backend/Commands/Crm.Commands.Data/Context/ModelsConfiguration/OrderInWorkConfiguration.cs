using Crm.Commands.Core.Clients;
using Crm.Commands.Core.Managers;
using Crm.Commands.Core.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Commands.Data.Context.ModelsConfiguration
{
    internal class OrderInWorkConfiguration : IEntityTypeConfiguration<OrderInWork>
    {
        public void Configure(EntityTypeBuilder<OrderInWork> builder)
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
            builder.HasOne<Client>()
                .WithMany(c => c.OrdersInWork)
                .HasForeignKey(x => x.ClientId)
                .IsRequired();
            builder.HasOne<Manager>()
                .WithMany(m => m.OrdersInWork)
                .HasForeignKey(x => x.ManagerId)
                .IsRequired();
        }
    }
}
