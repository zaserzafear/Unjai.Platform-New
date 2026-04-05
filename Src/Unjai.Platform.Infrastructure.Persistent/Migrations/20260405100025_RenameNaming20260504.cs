using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class RenameNaming20260504 : Migration
    {
        private static readonly string[] tenant_admin_role_permissions_columns = new[] { "role_id", "permission_id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_admin_refresh_tokens_tenants_admin_tenant_admin_id",
                table: "tenant_admin_refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_admin_tenants_admin_roles_role_id",
                table: "tenants_admin");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_admin_role_permissions_tenants_admin_permissions_pe~",
                table: "tenants_admin_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_admin_role_permissions_tenants_admin_roles_role_id",
                table: "tenants_admin_role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants_admin_roles",
                table: "tenants_admin_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants_admin_role_permissions",
                table: "tenants_admin_role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants_admin_permissions",
                table: "tenants_admin_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants_admin",
                table: "tenants_admin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jwt_signing_key",
                table: "jwt_signing_key");

            migrationBuilder.RenameTable(
                name: "tenants_admin_roles",
                newName: "tenant_admin_roles");

            migrationBuilder.RenameTable(
                name: "tenants_admin_role_permissions",
                newName: "tenant_admin_role_permissions");

            migrationBuilder.RenameTable(
                name: "tenants_admin_permissions",
                newName: "tenant_admin_permissions");

            migrationBuilder.RenameTable(
                name: "tenants_admin",
                newName: "tenant_admins");

            migrationBuilder.RenameTable(
                name: "jwt_signing_key",
                newName: "jwt_signing_keys");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_admin_roles_code",
                table: "tenant_admin_roles",
                newName: "IX_tenant_admin_roles_code");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_admin_role_permissions_permission_id",
                table: "tenant_admin_role_permissions",
                newName: "IX_tenant_admin_role_permissions_permission_id");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_admin_permissions_code",
                table: "tenant_admin_permissions",
                newName: "IX_tenant_admin_permissions_code");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_admin_username",
                table: "tenant_admins",
                newName: "IX_tenant_admins_username");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_admin_role_id",
                table: "tenant_admins",
                newName: "IX_tenant_admins_role_id");

            migrationBuilder.RenameIndex(
                name: "idx_tenants_admin_active_created_at_id",
                table: "tenant_admins",
                newName: "idx_tenant_admins_active_created_at_id");

            migrationBuilder.RenameIndex(
                name: "idx_jwt_signing_key_active",
                table: "jwt_signing_keys",
                newName: "idx_jwt_signing_keys_active");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "tenant_admin_refresh_tokens",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_admin_roles",
                table: "tenant_admin_roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_admin_role_permissions",
                table: "tenant_admin_role_permissions",
                columns: tenant_admin_role_permissions_columns);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_admin_permissions",
                table: "tenant_admin_permissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_admins",
                table: "tenant_admins",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jwt_signing_keys",
                table: "jwt_signing_keys",
                column: "key_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_admin_refresh_tokens_token",
                table: "tenant_admin_refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_admin_refresh_tokens_tenant_admins_tenant_admin_id",
                table: "tenant_admin_refresh_tokens",
                column: "tenant_admin_id",
                principalTable: "tenant_admins",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_admin_role_permissions_tenant_admin_permissions_perm~",
                table: "tenant_admin_role_permissions",
                column: "permission_id",
                principalTable: "tenant_admin_permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_admin_role_permissions_tenant_admin_roles_role_id",
                table: "tenant_admin_role_permissions",
                column: "role_id",
                principalTable: "tenant_admin_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_admins_tenant_admin_roles_role_id",
                table: "tenant_admins",
                column: "role_id",
                principalTable: "tenant_admin_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_admin_refresh_tokens_tenant_admins_tenant_admin_id",
                table: "tenant_admin_refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_admin_role_permissions_tenant_admin_permissions_perm~",
                table: "tenant_admin_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_admin_role_permissions_tenant_admin_roles_role_id",
                table: "tenant_admin_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_admins_tenant_admin_roles_role_id",
                table: "tenant_admins");

            migrationBuilder.DropIndex(
                name: "IX_tenant_admin_refresh_tokens_token",
                table: "tenant_admin_refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_admins",
                table: "tenant_admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_admin_roles",
                table: "tenant_admin_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_admin_role_permissions",
                table: "tenant_admin_role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_admin_permissions",
                table: "tenant_admin_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jwt_signing_keys",
                table: "jwt_signing_keys");

            migrationBuilder.RenameTable(
                name: "tenant_admins",
                newName: "tenants_admin");

            migrationBuilder.RenameTable(
                name: "tenant_admin_roles",
                newName: "tenants_admin_roles");

            migrationBuilder.RenameTable(
                name: "tenant_admin_role_permissions",
                newName: "tenants_admin_role_permissions");

            migrationBuilder.RenameTable(
                name: "tenant_admin_permissions",
                newName: "tenants_admin_permissions");

            migrationBuilder.RenameTable(
                name: "jwt_signing_keys",
                newName: "jwt_signing_key");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admins_username",
                table: "tenants_admin",
                newName: "IX_tenants_admin_username");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admins_role_id",
                table: "tenants_admin",
                newName: "IX_tenants_admin_role_id");

            migrationBuilder.RenameIndex(
                name: "idx_tenant_admins_active_created_at_id",
                table: "tenants_admin",
                newName: "idx_tenants_admin_active_created_at_id");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admin_roles_code",
                table: "tenants_admin_roles",
                newName: "IX_tenants_admin_roles_code");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admin_role_permissions_permission_id",
                table: "tenants_admin_role_permissions",
                newName: "IX_tenants_admin_role_permissions_permission_id");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_admin_permissions_code",
                table: "tenants_admin_permissions",
                newName: "IX_tenants_admin_permissions_code");

            migrationBuilder.RenameIndex(
                name: "idx_jwt_signing_keys_active",
                table: "jwt_signing_key",
                newName: "idx_jwt_signing_key_active");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "tenant_admin_refresh_tokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants_admin",
                table: "tenants_admin",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants_admin_roles",
                table: "tenants_admin_roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants_admin_role_permissions",
                table: "tenants_admin_role_permissions",
                columns: tenant_admin_role_permissions_columns);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants_admin_permissions",
                table: "tenants_admin_permissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jwt_signing_key",
                table: "jwt_signing_key",
                column: "key_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_admin_refresh_tokens_tenants_admin_tenant_admin_id",
                table: "tenant_admin_refresh_tokens",
                column: "tenant_admin_id",
                principalTable: "tenants_admin",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_admin_tenants_admin_roles_role_id",
                table: "tenants_admin",
                column: "role_id",
                principalTable: "tenants_admin_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_admin_role_permissions_tenants_admin_permissions_pe~",
                table: "tenants_admin_role_permissions",
                column: "permission_id",
                principalTable: "tenants_admin_permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_admin_role_permissions_tenants_admin_roles_role_id",
                table: "tenants_admin_role_permissions",
                column: "role_id",
                principalTable: "tenants_admin_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
