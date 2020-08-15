using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Changed_Duration_to_String : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParsedLogFile");

            migrationBuilder.CreateTable(
                name: "ParsedLogFile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArcVersion = table.Column<string>(nullable: true),
                    BossIcon = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    EliteInsightsVersion = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Gw2Build = table.Column<long>(nullable: false, defaultValue: 0L),
                    IsCM = table.Column<bool>(nullable: false, defaultValue: false),
                    Language = table.Column<string>(nullable: true),
                    LanguageID = table.Column<long>(nullable: false, defaultValue: 0L),
                    RecordedBy = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Success = table.Column<bool>(nullable: false, defaultValue: false),
                    TriggerID = table.Column<long>(nullable: false, defaultValue: 0L),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedLogFile", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParsedLogFile");

            migrationBuilder.CreateTable(
                name: "ParsedLogFile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArcVersion = table.Column<string>(nullable: true),
                    BossIcon = table.Column<string>(nullable: true),
                    Duration = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    EliteInsightsVersion = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Gw2Build = table.Column<long>(nullable: false, defaultValue: 0L),
                    IsCM = table.Column<bool>(nullable: false, defaultValue: false),
                    Language = table.Column<string>(nullable: true),
                    LanguageID = table.Column<long>(nullable: false, defaultValue: 0L),
                    RecordedBy = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Success = table.Column<bool>(nullable: false, defaultValue: false),
                    TriggerID = table.Column<long>(nullable: false, defaultValue: 0L),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedLogFile", x => x.ID);
                });
        }
    }
}
