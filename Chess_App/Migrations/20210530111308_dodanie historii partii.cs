using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess_App.Migrations
{
    public partial class dodaniehistoriipartii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chess_GameHistory",
                columns: table => new
                {
                    Chess_GameHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerID = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Opponnent = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    FEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Chess_AppUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chess_GameHistory", x => x.Chess_GameHistoryId);
                    table.ForeignKey(
                        name: "FK_Chess_GameHistory_AspNetUsers_Chess_AppUserId",
                        column: x => x.Chess_AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chess_GameHistory_Chess_AppUserId",
                table: "Chess_GameHistory",
                column: "Chess_AppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chess_GameHistory");
        }
    }
}
