﻿using Crm.Core.Managers;
using Crm.Core.Supervisors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Data.Context.ModelsConfiguration
{
    internal class SupervisorConfiguration : IEntityTypeConfiguration<Supervisor>
    {
        public void Configure(EntityTypeBuilder<Supervisor> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Managers)
                .WithOne()
                .HasForeignKey(m => m.SupervisorId)
                .IsRequired();
        }
    }
}
