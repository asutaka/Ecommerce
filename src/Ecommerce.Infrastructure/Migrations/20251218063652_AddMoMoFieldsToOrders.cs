using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoMoFieldsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 6, 36, 52, 546, DateTimeKind.Utc).AddTicks(5507), "$2a$11$IYeGEa2NM4p9Wr/sC1yIKeR7p16FOIWAhQOoGi1vW6xxQHTP0q/yi", new DateTime(2025, 12, 18, 6, 36, 52, 546, DateTimeKind.Utc).AddTicks(5515) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 18, 6, 35, 27, 976, DateTimeKind.Utc).AddTicks(8752), "$2a$11$Xp3G0mm/TmXu/luqyhxW6eUzcJsXr.n8xZ4DkK9o7aD4iA0f6meTa", new DateTime(2025, 12, 18, 6, 35, 27, 976, DateTimeKind.Utc).AddTicks(8757) });
        }
    }
}
