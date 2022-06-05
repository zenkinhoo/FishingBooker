using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class addIdsWhereMissed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CottageOwnerId",
                table: "Cottage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoatOwnerId",
                table: "Boat",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CottageOwnerId",
                table: "Cottage");

            migrationBuilder.DropColumn(
                name: "BoatOwnerId",
                table: "Boat");
        }
    }
}
