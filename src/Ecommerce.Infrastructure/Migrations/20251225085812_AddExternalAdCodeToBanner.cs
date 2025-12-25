using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalAdCodeToBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalAdCode",
                table: "Banners",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 8, 58, 11, 645, DateTimeKind.Utc).AddTicks(7639), "$2a$11$9nb/KdffsZNuD23kzOkTBO29Tzs659gKy7RADJ6zwrSaFCpgJUv1e", new DateTime(2025, 12, 25, 8, 58, 11, 645, DateTimeKind.Utc).AddTicks(7643) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalAdCode",
                table: "Banners");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 7, 35, 10, 320, DateTimeKind.Utc).AddTicks(9792), "$2a$11$Sv9RiTrkJ/H4DuipSKvit.71Oc4gs4/SfKbw5.1rxon7rNsMZd3mC", new DateTime(2025, 12, 25, 7, 35, 10, 320, DateTimeKind.Utc).AddTicks(9798) });
        }
    }
}
