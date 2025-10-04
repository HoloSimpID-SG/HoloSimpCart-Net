using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuTakingTooLong.Migrations
{
    /// <inheritdoc />
    public partial class CommandVCS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandVCS",
                columns: table => new
                {
                    command_name = table.Column<string>(type: "text", nullable: false),
                    version_hash = table.Column<string>(type: "text", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandVCS", x => x.command_name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandVCS");
        }
    }
}
