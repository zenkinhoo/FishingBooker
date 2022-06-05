using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class useremailtoappeals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "CottageAppeal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "BoatAppeal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "AdventureAppeal",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "CottageAppeal");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "BoatAppeal");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "AdventureAppeal");
        }
    }
}
