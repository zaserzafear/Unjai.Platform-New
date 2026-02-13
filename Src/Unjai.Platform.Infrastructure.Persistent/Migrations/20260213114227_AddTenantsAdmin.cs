using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantsAdmin : Migration
    {
        private static readonly string[] columns = new[] { "created_at", "id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants_admin_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_admin_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_admin_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_admin_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_admin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_admin", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenants_admin_tenants_admin_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "tenants_admin_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenants_admin_role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_admin_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_tenants_admin_role_permissions_tenants_admin_permissions_pe~",
                        column: x => x.permission_id,
                        principalTable: "tenants_admin_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tenants_admin_role_permissions_tenants_admin_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "tenants_admin_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_tenants_admin_active_created_at_id",
                table: "tenants_admin",
                columns: columns,
                filter: "\"is_deleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_admin_role_id",
                table: "tenants_admin",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_admin_username",
                table: "tenants_admin",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_admin_permissions_code",
                table: "tenants_admin_permissions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_admin_role_permissions_permission_id",
                table: "tenants_admin_role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_admin_roles_code",
                table: "tenants_admin_roles",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_admin");

            migrationBuilder.DropTable(
                name: "tenants_admin_role_permissions");

            migrationBuilder.DropTable(
                name: "tenants_admin_permissions");

            migrationBuilder.DropTable(
                name: "tenants_admin_roles");
        }
    }
}
