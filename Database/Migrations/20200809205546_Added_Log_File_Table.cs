using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Added_Log_File_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* 
             * NOTE: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations
             * https://sqlite.org/lang_altertable.html#otheralter
             * 
             * Normally you would transfer the Data of the old table to the new one, since you can't drop Primary Keys.
             * But the table changed so drastically, that it makes no sense to transfer it.
             * (Besides, migrations aren't even really needed at this point (if I'm not mistaken) and could be simplified by writing a 'normal' sql script instead.)
             */

            migrationBuilder.DropTable(
                name: "ParsedLogFiles");

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

            migrationBuilder.CreateTable(
                name: "LogFile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BossName = table.Column<string>(nullable: true),
                    Recorder = table.Column<string>(nullable: true),
                    ParsedLogFileID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogFile", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogFile_ParsedLogFile_ParsedLogFileID",
                        column: x => x.ParsedLogFileID,
                        principalTable: "ParsedLogFile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogPlayer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountName = table.Column<string>(nullable: true),
                    SubGroup = table.Column<int>(nullable: false),
                    HasCommander = table.Column<bool>(nullable: false),
                    Weapons = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Condition = table.Column<int>(nullable: false),
                    Concentration = table.Column<int>(nullable: false),
                    Healing = table.Column<int>(nullable: false),
                    Instance = table.Column<int>(nullable: false),
                    Toughness = table.Column<int>(nullable: false),
                    ParsedLogFileID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogPlayer", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogPlayer_ParsedLogFile_ParsedLogFileID",
                        column: x => x.ParsedLogFileID,
                        principalTable: "ParsedLogFile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogFile_ParsedLogFileID",
                table: "LogFile",
                column: "ParsedLogFileID");

            migrationBuilder.CreateIndex(
                name: "IX_LogPlayer_ParsedLogFileID",
                table: "LogPlayer",
                column: "ParsedLogFileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogFile");

            migrationBuilder.DropTable(
                name: "LogPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParsedLogFile",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "ArcVersion",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "BossIcon",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "EliteInsightsVersion",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "Gw2Build",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "IsCM",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "RecordedBy",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "ParsedLogFile");

            migrationBuilder.DropColumn(
                name: "TriggerID",
                table: "ParsedLogFile");

            migrationBuilder.RenameTable(
                name: "ParsedLogFile",
                newName: "ParsedLogFiles");

            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "ParsedLogFiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Json",
                table: "ParsedLogFiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recorder",
                table: "ParsedLogFiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParsedLogFiles",
                table: "ParsedLogFiles",
                column: "ID");
        }
    }
}
