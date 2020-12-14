using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParsedLogFile",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EliteInsightsVersion = table.Column<string>(nullable: true),
                    TriggerID = table.Column<long>(nullable: false),
                    BossName = table.Column<string>(nullable: true),
                    BossIcon = table.Column<string>(nullable: true),
                    ArcVersion = table.Column<string>(nullable: true),
                    Gw2Build = table.Column<long>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    LanguageID = table.Column<long>(nullable: false),
                    RecordedBy = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<string>(nullable: true),
                    Success = table.Column<bool>(nullable: false),
                    IsCM = table.Column<bool>(nullable: false),
                    DpsReportLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedLogFile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "LogPlayer",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountName = table.Column<string>(nullable: true),
                    SubGroup = table.Column<long>(nullable: false),
                    HasCommander = table.Column<bool>(nullable: false),
                    Profession = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Condition = table.Column<long>(nullable: false),
                    Concentration = table.Column<long>(nullable: false),
                    Healing = table.Column<long>(nullable: false),
                    Toughness = table.Column<long>(nullable: false),
                    Instance = table.Column<long>(nullable: false),
                    ParsedLogFileID = table.Column<long>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DpsTarget",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DPS = table.Column<long>(nullable: false),
                    Damage = table.Column<long>(nullable: false),
                    CondiDPS = table.Column<long>(nullable: false),
                    CondiDamage = table.Column<long>(nullable: false),
                    PowerDPS = table.Column<long>(nullable: false),
                    PowerDamage = table.Column<long>(nullable: false),
                    ActorDPS = table.Column<long>(nullable: false),
                    ActorDamage = table.Column<long>(nullable: false),
                    ActorCondiDPS = table.Column<long>(nullable: false),
                    ActorCondiDamage = table.Column<long>(nullable: false),
                    ActorPowerDPS = table.Column<long>(nullable: false),
                    ActorPowerDamage = table.Column<long>(nullable: false),
                    PlayerID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DpsTarget", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DpsTarget_LogPlayer_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "LogPlayer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DpsTarget_PlayerID",
                table: "DpsTarget",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_LogPlayer_ParsedLogFileID",
                table: "LogPlayer",
                column: "ParsedLogFileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DpsTarget");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "LogPlayer");

            migrationBuilder.DropTable(
                name: "ParsedLogFile");
        }
    }
}
