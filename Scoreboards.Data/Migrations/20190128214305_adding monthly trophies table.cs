using Microsoft.EntityFrameworkCore.Migrations;

namespace Scoreboards.Data.Migrations
{
    public partial class addingmonthlytrophiestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GameScoreUser02",
                table: "UserGames",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "GameScoreUser01",
                table: "UserGames",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GameScoreUser02",
                table: "UserGames",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "GameScoreUser01",
                table: "UserGames",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
