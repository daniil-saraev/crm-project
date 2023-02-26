using Crm.Commands.Core.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Commands.Data.Context.ModelsConfiguration
{
    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired();
            builder.OwnsOne(x => x.ContactInfo)
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();
            builder.OwnsOne(x => x.ContactInfo)
                .HasIndex(x => x.Email)
                .IsUnique();
            builder.HasMany(x => x.CompletedOrders)
                .WithOne()
                .HasForeignKey(o => o.ClientId)
                .IsRequired();
            builder.HasMany(x => x.CreatedOrders)
                .WithOne()
                .HasForeignKey(o => o.ClientId)
                .IsRequired();
            builder.HasMany(x => x.OrdersInWork)
                .WithOne()
                .HasForeignKey(o => o.ClientId)
                .IsRequired();
        }
    }
}
