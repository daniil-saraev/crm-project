using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Supervisors_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfoEmail = table.Column<string>(name: "ContactInfo_Email", type: "nvarchar(450)", nullable: false),
                    ContactInfoPhoneNumber = table.Column<string>(name: "ContactInfo_PhoneNumber", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompletedOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompletedOrders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompletedOrders_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatedOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatedOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatedOrders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersInWork",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Assigned = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersInWork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersInWork_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersInWork_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ContactInfo_Email",
                table: "Clients",
                column: "ContactInfo_Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ContactInfo_PhoneNumber",
                table: "Clients",
                column: "ContactInfo_PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ManagerId",
                table: "Clients",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedOrders_ClientId",
                table: "CompletedOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedOrders_ManagerId",
                table: "CompletedOrders",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedOrders_ClientId",
                table: "CreatedOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_SupervisorId",
                table: "Managers",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersInWork_ClientId",
                table: "OrdersInWork",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersInWork_ManagerId",
                table: "OrdersInWork",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedOrders");

            migrationBuilder.DropTable(
                name: "CreatedOrders");

            migrationBuilder.DropTable(
                name: "OrdersInWork");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Supervisors");
        }
    }
}
