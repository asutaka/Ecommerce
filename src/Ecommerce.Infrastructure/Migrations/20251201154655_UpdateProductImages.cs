using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
               name: "Images",
               table: "Products");
            migrationBuilder.AddColumn<List<string>>(
                name: "Images",
                table: "Products",
                type: "text[] NOT NULL DEFAULT '{}'",
                nullable: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c812781e-3cc3-4cef-a0aa-3b40ffde4f74"),
                column: "Images",
                value: new List<string> { "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10"),
                column: "Images",
                value: new List<string> { "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                column: "Images",
                value: new List<string> { "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "HeroImageUrl",
                table: "Products",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c812781e-3cc3-4cef-a0aa-3b40ffde4f74"),
                column: "HeroImageUrl",
                value: "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10"),
                column: "HeroImageUrl",
                value: "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                column: "HeroImageUrl",
                value: "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60");
        }
    }
}
