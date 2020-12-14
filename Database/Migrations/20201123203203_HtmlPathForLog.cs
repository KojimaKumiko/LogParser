using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class HtmlPathForLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HtmlPath",
                table: "ParsedLogFile",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlPath",
                table: "ParsedLogFile");
        }
    }
}
