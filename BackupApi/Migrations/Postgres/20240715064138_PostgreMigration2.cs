using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupApi.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class PostgreMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetServerName",
                table: "BackupHistory",
                newName: "BackupJobName");

            migrationBuilder.AddColumn<bool>(
                name: "IsUseScheduler",
                table: "BackupJob",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "BackupJobId",
                table: "BackupHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "BackupSchedulerId",
                table: "BackupHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BackupStatusId",
                table: "BackupHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUseScheduler",
                table: "BackupJob");

            migrationBuilder.DropColumn(
                name: "BackupSchedulerId",
                table: "BackupHistory");

            migrationBuilder.DropColumn(
                name: "BackupStatusId",
                table: "BackupHistory");

            migrationBuilder.RenameColumn(
                name: "BackupJobName",
                table: "BackupHistory",
                newName: "TargetServerName");

            migrationBuilder.AlterColumn<string>(
                name: "BackupJobId",
                table: "BackupHistory",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
