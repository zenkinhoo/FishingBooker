using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class advSpecOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdventureRealisationId",
                table: "AdventureSpecialOffer");

            migrationBuilder.AddColumn<string>(
                name: "AdventureId",
                table: "AdventureSpecialOffer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "AdventureSpecialOffer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "AdventureSpecialOffer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPersonCount",
                table: "AdventureSpecialOffer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "AdventureSpecialOffer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AdventureSpecialOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdventureId",
                table: "AdventureSpecialOffer");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "AdventureSpecialOffer");

            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "AdventureSpecialOffer");

            migrationBuilder.DropColumn(
                name: "MaxPersonCount",
                table: "AdventureSpecialOffer");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "AdventureSpecialOffer");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AdventureSpecialOffer");

            migrationBuilder.AddColumn<string>(
                name: "AdventureRealisationId",
                table: "AdventureSpecialOffer",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
