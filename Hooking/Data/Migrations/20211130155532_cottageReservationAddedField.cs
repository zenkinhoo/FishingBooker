using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class cottageReservationAddedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "CottageReservation",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "CottageReservation");
        }
    }
}
