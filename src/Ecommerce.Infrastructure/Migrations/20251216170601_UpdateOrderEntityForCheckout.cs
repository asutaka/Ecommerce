using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderEntityForCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Orders",
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
                keyValue: new Guid("049380e6-ca5e-4541-8807-fba311e86238"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("115c3164-77c3-41f7-94d6-6c7d72c31746"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("679f44de-7d11-48b2-b5fe-c55c6a707e16"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("b3056dc2-7b3e-4fdb-bb69-cf7d23a44437"));

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 16, 16, 23, 2, 914, DateTimeKind.Utc).AddTicks(5761), "$2a$11$MUiPpQXfT0mqmudoZTkFwe.DWSUvQoL1iBmgbjwPimfR7VQa85HWS", new DateTime(2025, 12, 16, 16, 23, 2, 914, DateTimeKind.Utc).AddTicks(5766) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("2fdf07d4-82d5-4c00-9260-439919367152"), "GIAM40K", new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 15, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), true, 500000m, new DateTime(2025, 12, 9, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), 100, 0 },
                    { new Guid("8b1d79bb-76cc-47df-96c8-288a05942235"), "FREESHIP", new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), true, 300000m, new DateTime(2025, 12, 6, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), 200, 0 },
                    { new Guid("8dd70497-2f23-42b8-b74b-93f0cd771827"), "GIAM100K", new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 14, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), true, 1500000m, new DateTime(2025, 12, 13, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), 30, 0 },
                    { new Guid("9f6172b5-f04a-446a-9e7f-7ceff2f69174"), "GIAM50K", new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 30, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), true, 800000m, new DateTime(2025, 12, 11, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), new DateTime(2025, 12, 16, 16, 23, 2, 793, DateTimeKind.Utc).AddTicks(7417), 50, 0 }
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
