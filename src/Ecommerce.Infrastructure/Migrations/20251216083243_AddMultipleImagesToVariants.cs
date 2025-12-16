using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleImagesToVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProductVariants");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "ProductVariants",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "ProductVariants");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProductVariants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
