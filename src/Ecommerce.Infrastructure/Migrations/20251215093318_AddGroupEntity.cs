using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("15738cbc-31c4-4d2c-8a8c-fedbc5b0bdd4"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("1f53f4c5-3f0a-409e-afd7-24891b1867fd"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("5ddfa261-c28d-401f-9890-9d8de000c1c9"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("c477cbfb-f831-47fc-b091-dd0fa892778b"));

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 9, 33, 16, 686, DateTimeKind.Utc).AddTicks(2487), "$2a$11$zqzS3ECHcThAXbD3RFAaHuB3x3a3JbLiKCcw4AYXqdvELzsEtz0gm", new DateTime(2025, 12, 15, 9, 33, 16, 686, DateTimeKind.Utc).AddTicks(2491) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("38cdf653-2d9c-405e-8949-ddac2e00a7ac"), "GIAM100K", new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 13, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), true, 1500000m, new DateTime(2025, 12, 12, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), 30, 0 },
                    { new Guid("7508f25e-79f7-4506-adaa-8c527848d031"), "FREESHIP", new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), true, 300000m, new DateTime(2025, 12, 5, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), 200, 0 },
                    { new Guid("bf1762dd-5d1e-4bd1-9ebf-a4c74315536d"), "GIAM40K", new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 14, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), true, 500000m, new DateTime(2025, 12, 8, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), 100, 0 },
                    { new Guid("cd706f41-e87c-4dae-ac23-2959c16b7ce5"), "GIAM50K", new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 29, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), true, 800000m, new DateTime(2025, 12, 10, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), new DateTime(2025, 12, 15, 9, 33, 16, 555, DateTimeKind.Utc).AddTicks(6156), 50, 0 }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Code",
                table: "Groups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IsActive",
                table: "Groups",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("38cdf653-2d9c-405e-8949-ddac2e00a7ac"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("7508f25e-79f7-4506-adaa-8c527848d031"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("bf1762dd-5d1e-4bd1-9ebf-a4c74315536d"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("cd706f41-e87c-4dae-ac23-2959c16b7ce5"));

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 6, 7, 50, 251, DateTimeKind.Utc).AddTicks(6520), "$2a$11$J8Dn0/CJzPYsHGmS1OVOBO0XW.FQyyjvDYqlUMXhKnwW1dUil311e", new DateTime(2025, 12, 15, 6, 7, 50, 251, DateTimeKind.Utc).AddTicks(6525) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("15738cbc-31c4-4d2c-8a8c-fedbc5b0bdd4"), "GIAM40K", new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 14, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), true, 500000m, new DateTime(2025, 12, 8, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), 100, 0 },
                    { new Guid("1f53f4c5-3f0a-409e-afd7-24891b1867fd"), "GIAM100K", new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 13, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), true, 1500000m, new DateTime(2025, 12, 12, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), 30, 0 },
                    { new Guid("5ddfa261-c28d-401f-9890-9d8de000c1c9"), "GIAM50K", new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 29, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), true, 800000m, new DateTime(2025, 12, 10, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), 50, 0 },
                    { new Guid("c477cbfb-f831-47fc-b091-dd0fa892778b"), "FREESHIP", new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), true, 300000m, new DateTime(2025, 12, 5, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), new DateTime(2025, 12, 15, 6, 7, 49, 947, DateTimeKind.Utc).AddTicks(141), 200, 0 }
                });

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
    }
}
