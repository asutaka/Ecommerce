using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 9, 32, 41, 670, DateTimeKind.Utc).AddTicks(6385), "$2a$11$5FM.llrThiwTTOnw9uJFMOTBWmEP2Q2cc8IWU4Z7prDLb5TNpzgau", new DateTime(2025, 12, 25, 9, 32, 41, 670, DateTimeKind.Utc).AddTicks(6389) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 8, 58, 11, 645, DateTimeKind.Utc).AddTicks(7639), "$2a$11$9nb/KdffsZNuD23kzOkTBO29Tzs659gKy7RADJ6zwrSaFCpgJUv1e", new DateTime(2025, 12, 25, 8, 58, 11, 645, DateTimeKind.Utc).AddTicks(7643) });
        }
    }
}
