using Microsoft.EntityFrameworkCore.Migrations;

namespace Scoreboards.Data.Migrations
{
    public partial class AddedGameDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameDescription",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameDescription",
                table: "Games");
        }
    }
}
