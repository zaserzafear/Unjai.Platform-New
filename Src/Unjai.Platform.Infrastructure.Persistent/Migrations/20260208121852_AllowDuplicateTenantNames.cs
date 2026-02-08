using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class AllowDuplicateTenantNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenants_name",
                table: "tenants");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                table: "tenants",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenants_name",
                table: "tenants");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                table: "tenants",
                column: "name",
                unique: true);
        }
    }
}
