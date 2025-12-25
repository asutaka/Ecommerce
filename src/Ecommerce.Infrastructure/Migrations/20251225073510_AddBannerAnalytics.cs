using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BannerAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BannerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    Clicks = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerAnalytics_Banners_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 7, 35, 10, 320, DateTimeKind.Utc).AddTicks(9792), "$2a$11$Sv9RiTrkJ/H4DuipSKvit.71Oc4gs4/SfKbw5.1rxon7rNsMZd3mC", new DateTime(2025, 12, 25, 7, 35, 10, 320, DateTimeKind.Utc).AddTicks(9798) });

            migrationBuilder.CreateIndex(
                name: "IX_BannerAnalytics_BannerId_Date",
                table: "BannerAnalytics",
                columns: new[] { "BannerId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BannerAnalytics_Date",
                table: "BannerAnalytics",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannerAnalytics");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 25, 6, 51, 56, 196, DateTimeKind.Utc).AddTicks(2728), "$2a$11$KIdjH3wDER1FtWiHjtyT1uDO7BIz/cBoPH4/Z6K54.7V4cBEuwEaa", new DateTime(2025, 12, 25, 6, 51, 56, 196, DateTimeKind.Utc).AddTicks(2732) });
        }
    }
}
