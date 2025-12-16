using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponFieldsToShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppliedCouponCode",
                table: "ShoppingCarts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ShoppingCarts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("01f41cee-0c75-46c0-8a23-074371db6d69"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("28e0dc19-b614-4eed-9dce-1f3e9084748a"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("2cac7280-5097-460d-a0c3-bd359a4c944e"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("3111f748-69de-4dfd-b39b-625e5c7460f1"));

            migrationBuilder.DropColumn(
                name: "AppliedCouponCode",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ShoppingCarts");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 16, 8, 32, 43, 215, DateTimeKind.Utc).AddTicks(7614), "$2a$11$RlbrW6.lmTAqbkWKm8I.UO9e4HOOZGrxnAJTMoADlEipWt4XyT6HG", new DateTime(2025, 12, 16, 8, 32, 43, 215, DateTimeKind.Utc).AddTicks(7624) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("128f9dd2-6143-405c-8496-73ba5ccf5094"), "GIAM100K", new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 14, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), true, 1500000m, new DateTime(2025, 12, 13, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), 30, 0 },
                    { new Guid("7375c68f-c51e-4e14-9e02-8850cdf7db32"), "FREESHIP", new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), true, 300000m, new DateTime(2025, 12, 6, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), 200, 0 },
                    { new Guid("8868080f-e9da-4f8e-a5cf-99c70f99c997"), "GIAM50K", new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 30, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), true, 800000m, new DateTime(2025, 12, 11, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), 50, 0 },
                    { new Guid("cd61ef7d-b271-4e73-9e7f-00b9b7df2503"), "GIAM40K", new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 15, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), true, 500000m, new DateTime(2025, 12, 9, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), new DateTime(2025, 12, 16, 8, 32, 43, 84, DateTimeKind.Utc).AddTicks(8813), 100, 0 }
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
