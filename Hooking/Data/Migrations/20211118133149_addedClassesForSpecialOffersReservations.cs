using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class addedClassesForSpecialOffersReservations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDetailsId",
                table: "CottageSpecialOffer");

            migrationBuilder.DropColumn(
                name: "UserDetailsId",
                table: "BoatSpecialOffer");

            migrationBuilder.CreateTable(
                name: "BoatSpecialOfferReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatSpecialOfferId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatSpecialOfferReservation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageSpecialOfferReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageSpecialOfferId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageSpecialOfferReservation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoatSpecialOfferReservation");

            migrationBuilder.DropTable(
                name: "CottageSpecialOfferReservation");

            migrationBuilder.AddColumn<string>(
                name: "UserDetailsId",
                table: "CottageSpecialOffer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserDetailsId",
                table: "BoatSpecialOffer",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
