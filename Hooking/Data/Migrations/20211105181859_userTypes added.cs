using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class userTypesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoatOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: false),
                    GradeCount = table.Column<int>(nullable: false),
                    IsCaptain = table.Column<bool>(nullable: false),
                    IsFirstOfficer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatOwner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: false),
                    GradeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageOwner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: false),
                    GradeCount = table.Column<int>(nullable: false),
                    Biography = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoatOwner");

            migrationBuilder.DropTable(
                name: "CottageOwner");

            migrationBuilder.DropTable(
                name: "Instructor");
        }
    }
}
