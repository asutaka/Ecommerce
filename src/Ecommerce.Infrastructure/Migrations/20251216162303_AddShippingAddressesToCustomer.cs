using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingAddressesToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress1",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress2",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress3",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("2fdf07d4-82d5-4c00-9260-439919367152"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("8b1d79bb-76cc-47df-96c8-288a05942235"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("8dd70497-2f23-42b8-b74b-93f0cd771827"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("9f6172b5-f04a-446a-9e7f-7ceff2f69174"));

            migrationBuilder.DropColumn(
                name: "ShippingAddress1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress2",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress3",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 16, 15, 33, 39, 82, DateTimeKind.Utc).AddTicks(3818), "$2a$11$At/Uais4Mz9JqFFvAqegPeKE6p9KPr3gMWlzFNBxswT8l03ixByAi", new DateTime(2025, 12, 16, 15, 33, 39, 82, DateTimeKind.Utc).AddTicks(3824) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("01f41cee-0c75-46c0-8a23-074371db6d69"), "GIAM50K", new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 30, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), true, 800000m, new DateTime(2025, 12, 11, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), 50, 0 },
                    { new Guid("28e0dc19-b614-4eed-9dce-1f3e9084748a"), "GIAM40K", new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 15, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), true, 500000m, new DateTime(2025, 12, 9, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), 100, 0 },
                    { new Guid("2cac7280-5097-460d-a0c3-bd359a4c944e"), "GIAM100K", new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 14, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), true, 1500000m, new DateTime(2025, 12, 13, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), 30, 0 },
                    { new Guid("3111f748-69de-4dfd-b39b-625e5c7460f1"), "FREESHIP", new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), true, 300000m, new DateTime(2025, 12, 6, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), new DateTime(2025, 12, 16, 15, 33, 38, 956, DateTimeKind.Utc).AddTicks(852), 200, 0 }
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
