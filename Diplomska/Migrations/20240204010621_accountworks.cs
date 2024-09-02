using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomska.Migrations
{
    public partial class accountworks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Guest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Guest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Used",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Guest");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Guest");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "AspNetUsers");
        }
    }
}
