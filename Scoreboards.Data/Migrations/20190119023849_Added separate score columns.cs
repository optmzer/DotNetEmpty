using Microsoft.EntityFrameworkCore.Migrations;

namespace Scoreboards.Data.Migrations
{
    public partial class Addedseparatescorecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameScoreUser01",
                table: "UserGames",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GameScoreUser02",
                table: "UserGames",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "User_01_Awarder_Points",
                table: "UserGames",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "User_02_Awarder_Points",
                table: "UserGames",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DrawPoints",
                table: "Games",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LossPoints",
                table: "Games",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinPoints",
                table: "Games",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameScoreUser01",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "GameScoreUser02",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "User_01_Awarder_Points",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "User_02_Awarder_Points",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "DrawPoints",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LossPoints",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinPoints",
                table: "Games");
        }
    }
}
