using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupApi.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class PostgreMigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "SchedulerClockTimeSet",
                table: "BackupScheduler",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "BackupJobId",
                table: "BackupScheduler",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SchedulerClockTimeSet",
                table: "BackupScheduler",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "BackupJobId",
                table: "BackupScheduler",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
