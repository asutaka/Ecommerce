using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRetryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPaymentAttempt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryScheduledAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentAttempts",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 7, 41, 33, 842, DateTimeKind.Utc).AddTicks(3437), "$2a$11$nlKvehOEts.n9GEB24oVRu3Ox/HS9v0WiqbZGfTXPabxW3P.67vAq", new DateTime(2025, 12, 18, 7, 41, 33, 842, DateTimeKind.Utc).AddTicks(3445) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPaymentAttempt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NextRetryScheduledAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentAttempts",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 6, 51, 36, 645, DateTimeKind.Utc).AddTicks(5282), "$2a$11$7fPkou.X6h.cL8IIlUE0wOqfnoslvp.THs6Wm.m8oukV367CXi7oy", new DateTime(2025, 12, 18, 6, 51, 36, 645, DateTimeKind.Utc).AddTicks(5298) });
        }
    }
}
