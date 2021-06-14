using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess_App.Migrations
{
    public partial class Sekwencja : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MoveSequence",
                table: "Chess_GameHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveSequence",
                table: "Chess_GameHistory");
        }
    }
}
