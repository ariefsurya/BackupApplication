using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupApi.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class PostgreMigration6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "BackupHistory",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "BackupHistory");
        }
    }
}
