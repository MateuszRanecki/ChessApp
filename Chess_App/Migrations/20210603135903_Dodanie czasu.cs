using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess_App.Migrations
{
    public partial class Dodanieczasu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GameDate",
                table: "Chess_GameHistory",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameDate",
                table: "Chess_GameHistory");
        }
    }
}
