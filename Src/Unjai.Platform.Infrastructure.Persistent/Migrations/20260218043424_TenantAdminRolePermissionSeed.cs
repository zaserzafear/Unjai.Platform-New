using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class TenantAdminRolePermissionSeed : Migration
    {
        private static readonly string[] columns = new[] { "permission_id", "role_id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants_admin_role_permissions",
                columns: columns,
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "tenants_admin_role_permissions",
                keyColumns: columns,
                keyValues: new object[] { 4, 2 });
        }
    }
}
