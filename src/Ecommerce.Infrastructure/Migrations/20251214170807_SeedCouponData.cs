using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCouponData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 14, 17, 8, 6, 756, DateTimeKind.Utc).AddTicks(3484), "$2a$11$VNfSBPpUBCUBumss5lVkLeY8BKndFNs/fj/Iya9Dx0ICSoHCw6Xsq", new DateTime(2025, 12, 14, 17, 8, 6, 756, DateTimeKind.Utc).AddTicks(3489) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("6f4d0420-a892-4314-be9b-46e28553b18c"), "GIAM40K", new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 13, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), true, 500000m, new DateTime(2025, 12, 7, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), 100, 0 },
                    { new Guid("c1ffec36-0b3d-43c9-9a3e-f0f5caa8a047"), "GIAM50K", new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 28, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), true, 800000m, new DateTime(2025, 12, 9, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), 50, 0 },
                    { new Guid("c67947f3-1662-4cdf-878b-30dc6f751107"), "FREESHIP", new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), true, 300000m, new DateTime(2025, 12, 4, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), 200, 0 },
                    { new Guid("d5eba7ed-d836-460e-9c6c-d47f595663e6"), "GIAM100K", new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 12, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), true, 1500000m, new DateTime(2025, 12, 11, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), new DateTime(2025, 12, 14, 17, 8, 6, 243, DateTimeKind.Utc).AddTicks(2866), 30, 0 }
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("6f4d0420-a892-4314-be9b-46e28553b18c"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("c1ffec36-0b3d-43c9-9a3e-f0f5caa8a047"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("c67947f3-1662-4cdf-878b-30dc6f751107"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("d5eba7ed-d836-460e-9c6c-d47f595663e6"));

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 14, 17, 5, 53, 399, DateTimeKind.Utc).AddTicks(402), "$2a$11$p76vBaCoVsibTIETs8GWh.5qse8Gc.vK5ySDR0w1z2mSfXb2n2f9C", new DateTime(2025, 12, 14, 17, 5, 53, 399, DateTimeKind.Utc).AddTicks(408) });

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
