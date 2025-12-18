using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 6, 51, 36, 645, DateTimeKind.Utc).AddTicks(5282), "$2a$11$7fPkou.X6h.cL8IIlUE0wOqfnoslvp.THs6Wm.m8oukV367CXi7oy", new DateTime(2025, 12, 18, 6, 51, 36, 645, DateTimeKind.Utc).AddTicks(5298) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 6, 36, 52, 546, DateTimeKind.Utc).AddTicks(5507), "$2a$11$IYeGEa2NM4p9Wr/sC1yIKeR7p16FOIWAhQOoGi1vW6xxQHTP0q/yi", new DateTime(2025, 12, 18, 6, 36, 52, 546, DateTimeKind.Utc).AddTicks(5515) });
        }
    }
}
