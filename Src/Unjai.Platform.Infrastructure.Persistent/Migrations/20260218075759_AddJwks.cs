using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unjai.Platform.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class AddJwks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jwt_signing_key",
                columns: table => new
                {
                    key_id = table.Column<string>(type: "text", nullable: false),
                    public_key_pem = table.Column<string>(type: "text", nullable: false),
                    private_key_pem = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jwt_signing_key", x => x.key_id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_jwt_signing_key_active",
                table: "jwt_signing_key",
                column: "is_active",
                unique: true,
                filter: "\"is_active\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jwt_signing_key");
        }
    }
}
