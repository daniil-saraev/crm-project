// <auto-generated />
using System;
using Crm.Commands.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Crm.Data.Context.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Crm.Core.Clients.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ManagerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Crm.Core.Managers.Manager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SupervisorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SupervisorId");

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("Crm.Core.Orders.CompletedOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ManagerId");

                    b.ToTable("CompletedOrders");
                });

            modelBuilder.Entity("Crm.Core.Orders.CreatedOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset(0)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("CreatedOrders");
                });

            modelBuilder.Entity("Crm.Core.Orders.OrderInWork", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Assigned")
                        .HasColumnType("datetimeoffset(0)");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset(0)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ManagerId");

                    b.ToTable("OrdersInWork");
                });

            modelBuilder.Entity("Crm.Core.Supervisors.Supervisor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Supervisors");
                });

            modelBuilder.Entity("Crm.Core.Clients.Client", b =>
                {
                    b.HasOne("Crm.Core.Managers.Manager", null)
                        .WithMany("Clients")
                        .HasForeignKey("ManagerId");

                    b.OwnsOne("Crm.Shared.Models.ContactInfo", "ContactInfo", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("PhoneNumber")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.HasKey("ClientId");

                            b1.HasIndex("Email")
                                .IsUnique();

                            b1.HasIndex("PhoneNumber")
                                .IsUnique();

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.Navigation("ContactInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("Crm.Core.Managers.Manager", b =>
                {
                    b.HasOne("Crm.Core.Supervisors.Supervisor", null)
                        .WithMany("Managers")
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Crm.Core.Orders.CompletedOrder", b =>
                {
                    b.HasOne("Crm.Core.Clients.Client", null)
                        .WithMany("CompletedOrders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Crm.Core.Managers.Manager", null)
                        .WithMany("CompletedOrders")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Crm.Core.Orders.CreatedOrder", b =>
                {
                    b.HasOne("Crm.Core.Clients.Client", null)
                        .WithMany("CreatedOrders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Crm.Core.Orders.OrderInWork", b =>
                {
                    b.HasOne("Crm.Core.Clients.Client", null)
                        .WithMany("OrdersInWork")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Crm.Core.Managers.Manager", null)
                        .WithMany("OrdersInWork")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Crm.Core.Clients.Client", b =>
                {
                    b.Navigation("CompletedOrders");

                    b.Navigation("CreatedOrders");

                    b.Navigation("OrdersInWork");
                });

            modelBuilder.Entity("Crm.Core.Managers.Manager", b =>
                {
                    b.Navigation("Clients");

                    b.Navigation("CompletedOrders");

                    b.Navigation("OrdersInWork");
                });

            modelBuilder.Entity("Crm.Core.Supervisors.Supervisor", b =>
                {
                    b.Navigation("Managers");
                });
#pragma warning restore 612, 618
        }
    }
}
