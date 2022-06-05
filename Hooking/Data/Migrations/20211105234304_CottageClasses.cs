using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class CottageClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelationPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    FreeUntil = table.Column<int>(nullable: false),
                    PenaltyPercentage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelationPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cottage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RoomCount = table.Column<int>(nullable: false),
                    Area = table.Column<int>(nullable: false),
                    AverageGrade = table.Column<double>(nullable: false),
                    GradeCount = table.Column<int>(nullable: false),
                    CancelationPolicyId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cottage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageOwnerReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageOwnerId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageOwnerReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    PersonCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageReservation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageReservationReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ReservationId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    DidntShow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageReservationReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageRoom",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BedCount = table.Column<int>(nullable: false),
                    AirCondition = table.Column<bool>(nullable: false),
                    TV = table.Column<bool>(nullable: false),
                    Balcony = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottageRoom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottageSpecialOffer",
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
                    table.PrimaryKey("PK_CottageSpecialOffer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CottagesRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    CottageRoomId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CottagesRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    Parking = table.Column<bool>(nullable: false),
                    Wifi = table.Column<bool>(nullable: false),
                    Heating = table.Column<bool>(nullable: false),
                    BarbecueFacilities = table.Column<bool>(nullable: false),
                    OnlineCheckin = table.Column<bool>(nullable: false),
                    Jacuzzi = table.Column<bool>(nullable: false),
                    SeaView = table.Column<bool>(nullable: false),
                    MountainView = table.Column<bool>(nullable: false),
                    Kitchen = table.Column<bool>(nullable: false),
                    WashingMachine = table.Column<bool>(nullable: false),
                    AirportShuttle = table.Column<bool>(nullable: false),
                    IndoorPool = table.Column<bool>(nullable: false),
                    OutdoorPool = table.Column<bool>(nullable: false),
                    StockedBar = table.Column<bool>(nullable: false),
                    Garden = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HouseRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CottageId = table.Column<string>(nullable: true),
                    PetFriendly = table.Column<bool>(nullable: false),
                    NonSmoking = table.Column<bool>(nullable: false),
                    CheckinTime = table.Column<DateTime>(nullable: false),
                    CheckoutTime = table.Column<DateTime>(nullable: false),
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
                name: "CancelationPolicy");

            migrationBuilder.DropTable(
                name: "Cottage");

            migrationBuilder.DropTable(
                name: "CottageOwnerReview");

            migrationBuilder.DropTable(
                name: "CottageReservation");

            migrationBuilder.DropTable(
                name: "CottageReservationReview");

            migrationBuilder.DropTable(
                name: "CottageReview");

            migrationBuilder.DropTable(
                name: "CottageRoom");

            migrationBuilder.DropTable(
                name: "CottageSpecialOffer");

            migrationBuilder.DropTable(
                name: "CottagesRooms");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "HouseRules");
        }
    }
}
