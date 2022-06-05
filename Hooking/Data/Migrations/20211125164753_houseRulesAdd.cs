using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class houseRulesAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HouseRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PetFriendly = table.Column<bool>(nullable: false),
                    NonSmoking = table.Column<bool>(nullable: false),
                    CheckInTime = table.Column<int>(nullable: false),
                    CheckOutTime = table.Column<int>(nullable: false),
                    AgeRestriction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseRules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HouseRules");
        }
    }
}
