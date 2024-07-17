using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackupApi.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class PostgreMigration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackupScheduler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BackupJobId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    BackupSchedulerType = table.Column<int>(type: "integer", nullable: false),
                    SchedulerDateDaySet = table.Column<string>(type: "text", nullable: false),
                    SchedulerClockTimeSet = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SchedulerStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupScheduler", x => x.Id);
                });

            migrationBuilder.AlterColumn<string>(
                name: "TargetFileName",
                table: "TargetBackup",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackupScheduler");

            migrationBuilder.AlterColumn<string>(
                name: "TargetFileName",
                table: "TargetBackup",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
        /// <inheritdoc />
        //protected override void Up(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.AlterColumn<string>(
        //        name: "TargetFileName",
        //        table: "TargetBackup",
        //        type: "text",
        //        nullable: true,
        //        oldClrType: typeof(string),
        //        oldType: "text");

        //    migrationBuilder.AlterColumn<TimeSpan>(
        //        name: "SchedulerClockTimeSet",
        //        table: "BackupScheduler",
        //        type: "interval",
        //        nullable: false,
        //        oldClrType: typeof(TimeOnly),
        //        oldType: "time without time zone");
        //}

        ///// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.AlterColumn<string>(
        //        name: "TargetFileName",
        //        table: "TargetBackup",
        //        type: "text",
        //        nullable: false,
        //        defaultValue: "",
        //        oldClrType: typeof(string),
        //        oldType: "text",
        //        oldNullable: true);

        //    migrationBuilder.AlterColumn<TimeOnly>(
        //        name: "SchedulerClockTimeSet",
        //        table: "BackupScheduler",
        //        type: "time without time zone",
        //        nullable: false,
        //        oldClrType: typeof(TimeSpan),
        //        oldType: "interval");
        //}
    }
}
