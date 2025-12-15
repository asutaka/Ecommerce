using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Products_Categories_CategoryId",
            //     table: "Products");
            
            migrationBuilder.Sql("ALTER TABLE \"Products\" DROP CONSTRAINT IF EXISTS \"FK_Products_Categories_CategoryId\";");

            // migrationBuilder.DropIndex(
            //     name: "IX_Products_CategoryId",
            //     table: "Products");
             migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Products_CategoryId\";");

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

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryCategoryId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                columns: new[] { "Images", "PrimaryCategoryId" },
                values: new object[] { new List<string> { "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60" }, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10"),
                columns: new[] { "Images", "PrimaryCategoryId" },
                values: new object[] { new List<string> { "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60" }, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                columns: new[] { "Images", "PrimaryCategoryId" },
                values: new object[] { new List<string> { "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60" }, null });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PrimaryCategoryId",
                table: "Products",
                column: "PrimaryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_PrimaryCategoryId",
                table: "Products",
                column: "PrimaryCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_PrimaryCategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Products_PrimaryCategoryId",
                table: "Products");

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

            migrationBuilder.DropColumn(
                name: "PrimaryCategoryId",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                columns: new[] { "CategoryId", "Images" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new List<string> { "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60" } });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10"),
                columns: new[] { "CategoryId", "Images" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new List<string> { "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60" } });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                columns: new[] { "CategoryId", "Images" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new List<string> { "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60" } });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
