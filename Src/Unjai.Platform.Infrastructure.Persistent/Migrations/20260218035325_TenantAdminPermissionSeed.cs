using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class TenantAdminPermissionSeed : Migration
    {
        private static readonly string[] columns = new[] { "id", "code", "name" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants_admin_permissions",
                columns: columns,
                values: new object[,]
                {
                    { 1, "READTENANTS", "Read Tenants" },
                    { 2, "CREATETENANTS", "Create Tenants" },
                    { 3, "UPDATETENANTS", "Update Tenants" },
                    { 4, "DELETETENANTS", "Delete Tenants" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tenants_admin_permissions",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tenants_admin_permissions",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "tenants_admin_permissions",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "tenants_admin_permissions",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
