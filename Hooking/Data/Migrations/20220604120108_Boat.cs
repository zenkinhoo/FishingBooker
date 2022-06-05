using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class Boat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasSubscribers",
                table: "Cottage",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasSubscribers",
                table: "Boat",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasSubscribers",
                table: "Adventure",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasSubscribers",
                table: "Cottage");

            migrationBuilder.DropColumn(
                name: "hasSubscribers",
                table: "Boat");

            migrationBuilder.DropColumn(
                name: "hasSubscribers",
                table: "Adventure");
        }
    }
}
