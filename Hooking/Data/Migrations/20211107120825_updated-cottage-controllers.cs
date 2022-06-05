using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class updatedcottagecontrollers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CottageId",
                table: "HouseRules");

            migrationBuilder.DropColumn(
                name: "CottageId",
                table: "Facilities");

            migrationBuilder.CreateTable(
                name: "CottagesFacilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    FacilitiesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottagesFacilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottagesHouseRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    HouseRulesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottagesHouseRules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CottagesFacilities");

            migrationBuilder.DropTable(
                name: "CottagesHouseRules");

            migrationBuilder.AddColumn<string>(
                name: "CottageId",
                table: "HouseRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CottageId",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
