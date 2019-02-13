using Microsoft.EntityFrameworkCore.Migrations;

namespace Scoreboards.Data.Migrations
{
    public partial class AddedApologisedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameScore",
                table: "UserGames");

            migrationBuilder.AddColumn<bool>(
                name: "Apologised",
                table: "UserGames",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apologised",
                table: "UserGames");

            migrationBuilder.AddColumn<string>(
                name: "GameScore",
                table: "UserGames",
                nullable: true);
        }
    }
}
