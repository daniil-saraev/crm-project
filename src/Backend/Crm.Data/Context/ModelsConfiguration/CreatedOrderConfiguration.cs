using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Data.Context.ModelsConfiguration
{
    internal class CreatedOrderConfiguration : IEntityTypeConfiguration<CreatedOrder>
    {
        public void Configure(EntityTypeBuilder<CreatedOrder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Created)
                .IsRequired()
                .HasColumnType("datetimeoffset(0)");
            builder.Property(x => x.Description)
                .IsRequired();
            builder.HasOne<Client>()
                .WithMany(c => c.CreatedOrders)
                .HasForeignKey(x => x.ClientId)
                .IsRequired();
        }
    }
}
