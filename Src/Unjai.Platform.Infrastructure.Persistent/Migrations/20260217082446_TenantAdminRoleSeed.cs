using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class TenantAdminRoleSeed : Migration
    {
        private static readonly string[] columns = new[] { "id", "code", "name" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants_admin_roles",
                columns: columns,
                values: new object[,]
                {
                    { 1, "SUPERADMIN", "Super Admin" },
                    { 2, "ADMIN", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tenants_admin_roles",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tenants_admin_roles",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
