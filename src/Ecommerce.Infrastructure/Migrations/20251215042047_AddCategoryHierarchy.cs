using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("3e752b0f-acf1-429f-bfd8-0bd8885bf98f"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("969abe04-bdf3-43d5-bbf5-16717578bcef"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("a095ce13-fefa-41d8-b520-c24182024737"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("bf149b6b-faf9-42a6-961b-7601c0ea0f3b"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 4, 20, 47, 405, DateTimeKind.Utc).AddTicks(5881), "$2a$11$64WSIQ1w06UOix/2NcKxZOznVwPvMV1a7QVSbEJaLFmtNbVejC8C2", new DateTime(2025, 12, 15, 4, 20, 47, 405, DateTimeKind.Utc).AddTicks(5884) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("25d75562-ce14-455f-86fc-eb54d4636319"), "FREESHIP", new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), true, 300000m, new DateTime(2025, 12, 5, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), 200, 0 },
                    { new Guid("83434861-f0ee-4be2-8273-9bcd6e231a74"), "GIAM40K", new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 14, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), true, 500000m, new DateTime(2025, 12, 8, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), 100, 0 },
                    { new Guid("838ed180-5318-40c5-8303-e28e7e20eae2"), "GIAM100K", new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 13, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), true, 1500000m, new DateTime(2025, 12, 12, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), 30, 0 },
                    { new Guid("a3efc911-b4f2-4cdb-acfb-0df721793598"), "GIAM50K", new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 29, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), true, 800000m, new DateTime(2025, 12, 10, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), new DateTime(2025, 12, 15, 4, 20, 47, 102, DateTimeKind.Utc).AddTicks(816), 50, 0 }
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
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("25d75562-ce14-455f-86fc-eb54d4636319"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("83434861-f0ee-4be2-8273-9bcd6e231a74"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("838ed180-5318-40c5-8303-e28e7e20eae2"));

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("a3efc911-b4f2-4cdb-acfb-0df721793598"));

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 4, 13, 15, 820, DateTimeKind.Utc).AddTicks(8620), "$2a$11$yELtFxlbPAYwezMgjlWX.OfXOMpRjekvnUheBcb79WriCjH8YjmLq", new DateTime(2025, 12, 15, 4, 13, 15, 820, DateTimeKind.Utc).AddTicks(8629) });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("3e752b0f-acf1-429f-bfd8-0bd8885bf98f"), "GIAM40K", new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), "Giảm 40.000đ cho đơn hàng từ 500.000đ", 40000m, null, new DateTime(2026, 1, 14, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), true, 500000m, new DateTime(2025, 12, 8, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), 100, 0 },
                    { new Guid("969abe04-bdf3-43d5-bbf5-16717578bcef"), "FREESHIP", new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), "Miễn phí vận chuyển cho đơn hàng từ 300.000đ", 30000m, null, new DateTime(2026, 3, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), true, 300000m, new DateTime(2025, 12, 5, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), 200, 0 },
                    { new Guid("a095ce13-fefa-41d8-b520-c24182024737"), "GIAM50K", new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), "Giảm 50.000đ cho đơn hàng từ 800.000đ", 50000m, null, new DateTime(2026, 1, 29, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), true, 800000m, new DateTime(2025, 12, 10, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), 50, 0 },
                    { new Guid("bf149b6b-faf9-42a6-961b-7601c0ea0f3b"), "GIAM100K", new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), "Giảm 100.000đ cho đơn hàng từ 1.500.000đ", 100000m, null, new DateTime(2026, 2, 13, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), true, 1500000m, new DateTime(2025, 12, 12, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), new DateTime(2025, 12, 15, 4, 13, 15, 690, DateTimeKind.Utc).AddTicks(6036), 30, 0 }
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
