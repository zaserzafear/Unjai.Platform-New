using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unjai.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OutboxMessage : Migration
    {
        private static readonly string[] columns = new[] { "occurred_on_utc", "processed_on_utc" };
        private static readonly string[] value = new[] { "id", "type", "content" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_outbox_messages_unprocessed",
                table: "outbox_messages",
                columns: columns,
                filter: "\"processed_on_utc\" IS NULL")
                .Annotation("Npgsql:IndexInclude", value);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");
        }
    }
}
