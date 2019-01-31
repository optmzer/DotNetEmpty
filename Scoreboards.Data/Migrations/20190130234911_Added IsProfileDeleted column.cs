using Microsoft.EntityFrameworkCore.Migrations;

namespace Scoreboards.Data.Migrations
{
    public partial class AddedIsProfileDeletedcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfileDeleted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfileDeleted",
                table: "AspNetUsers");
        }
    }
}
