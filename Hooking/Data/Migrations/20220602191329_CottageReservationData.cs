using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class CottageReservationData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "instructorId",
                table: "FilteredInstructors",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CottageReservationData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    MaxPersonCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageReservationData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CottageReservationData");

            migrationBuilder.DropColumn(
                name: "instructorId",
                table: "FilteredInstructors");
        }
    }
}
