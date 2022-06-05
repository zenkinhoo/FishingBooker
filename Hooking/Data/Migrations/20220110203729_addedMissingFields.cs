using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class addedMissingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "CottageSpecialOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "CottageSpecialOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CottageAppeal",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "BoatSpecialOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "BoatSpecialOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BoatAppeal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AdventureAppeal",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "CottageSpecialOffer");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "CottageSpecialOffer");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CottageAppeal");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "BoatSpecialOffer");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "BoatSpecialOffer");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "BoatAppeal");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AdventureAppeal");
        }
    }
}
