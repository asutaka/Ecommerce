using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManuallyAddMoMoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""MoMoRequestId"" text NULL;
                ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""MoMoTransactionId"" text NULL;
                ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""PaymentDate"" timestamp with time zone NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Orders"" DROP COLUMN IF EXISTS ""MoMoRequestId"";
                ALTER TABLE ""Orders"" DROP COLUMN IF EXISTS ""MoMoTransactionId"";
                ALTER TABLE ""Orders"" DROP COLUMN IF EXISTS ""PaymentDate"";
            ");
        }
    }
}
