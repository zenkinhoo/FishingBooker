using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class FilteredCottages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdventureReservationFilter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    startDate = table.Column<DateTime>(nullable: false),
                    endDate = table.Column<DateTime>(nullable: false),
                    price = table.Column<double>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: true),
                    MaxPersonCount = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureReservationFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilteredCottages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    RoomCount = table.Column<int>(nullable: false),
                    Area = table.Column<int>(nullable: false),
                    AverageGrade = table.Column<double>(nullable: false),
                    GradeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredCottages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdventureReservationFilter");

            migrationBuilder.DropTable(
                name: "FilteredCottages");
        }
    }
}
