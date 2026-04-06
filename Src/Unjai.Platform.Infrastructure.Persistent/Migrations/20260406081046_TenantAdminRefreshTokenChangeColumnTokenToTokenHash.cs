using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class TenantAdminRefreshTokenChangeColumnTokenToTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token",
                table: "tenant_admin_refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admin_refresh_tokens_token",
                table: "tenant_admin_refresh_tokens",
                newName: "IX_tenant_admin_refresh_tokens_token_hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "tenant_admin_refresh_tokens",
                newName: "token");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admin_refresh_tokens_token_hash",
                table: "tenant_admin_refresh_tokens",
                newName: "IX_tenant_admin_refresh_tokens_token");
        }
    }
}
