using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class DpsTargetAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DpsTarget");
        }
    }
}
