using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class creationModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonCount",
                table: "CottageReservation");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CottageSpecialOffer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceivedPenalty",
                table: "CottageReservationReview",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPersonCount",
                table: "CottageReservation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RegularPrice",
                table: "Cottage",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeekendPrice",
                table: "Cottage",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Adventure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    InstructorId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MaxPersonCount = table.Column<int>(nullable: false),
                    CancelationPolicy = table.Column<string>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    AdventureId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureFavorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureFishingEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureId = table.Column<string>(nullable: true),
                    FishingEquipmentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureFishingEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureFishingTechniques",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureId = table.Column<string>(nullable: true),
                    FishingTechniquesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureFishingTechniques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureRealisation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureId = table.Column<string>(nullable: true),
                    Duration = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureRealisation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureRealisationId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureReservation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureReservationReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureReservationId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    DidntShow = table.Column<bool>(nullable: false),
                    ReceivedPenalty = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureReservationReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ChildFriendly = table.Column<bool>(nullable: false),
                    YouKeepCatch = table.Column<bool>(nullable: false),
                    CatchAndReleaseAllowed = table.Column<bool>(nullable: false),
                    CabinSmoking = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventuresAdventureRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureId = table.Column<string>(nullable: true),
                    AdventureRulesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventuresAdventureRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdventureSpecialOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AdventureRealisationId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureSpecialOffer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Gps = table.Column<bool>(nullable: false),
                    Radar = table.Column<bool>(nullable: false),
                    VhrRadio = table.Column<bool>(nullable: false),
                    Sonar = table.Column<bool>(nullable: false),
                    FishFinder = table.Column<bool>(nullable: false),
                    WiFi = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boat",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Length = table.Column<int>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    EngineNumber = table.Column<string>(nullable: true),
                    EnginePower = table.Column<int>(nullable: false),
                    MaxSpeed = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CancelationPolicyId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AverageGrade = table.Column<double>(nullable: false),
                    RegularPrice = table.Column<double>(nullable: false),
                    WeekendPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatAmenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    AmanitiesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatAmenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    BoatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatFavorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatFishingEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    FishingEquipment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatFishingEquipment", x => x.Id);
                });

           /* migrationBuilder.CreateTable(
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
                });*/

            migrationBuilder.CreateTable(
                name: "BoatOwnerReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatOwnerId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatOwnerReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    PersonCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatReservation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatReservationReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatReservationId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    DidntShow = table.Column<bool>(nullable: false),
                    ReceivedPenalty = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatReservationReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ChildFriendly = table.Column<bool>(nullable: false),
                    YouKeepCatch = table.Column<bool>(nullable: false),
                    CatchAndReleaseAllowed = table.Column<bool>(nullable: false),
                    CabinSmoking = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatsBoatRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    BoatRulesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatsBoatRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoatSpecialOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    BoatId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    MaxPersonCount = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatSpecialOffer", x => x.Id);
                });

           /* migrationBuilder.CreateTable(
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
                });*/

            migrationBuilder.CreateTable(
                name: "FishingEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    LiveBite = table.Column<bool>(nullable: false),
                    FlyFishingGear = table.Column<bool>(nullable: false),
                    Lures = table.Column<bool>(nullable: false),
                    RodsReelsTackle = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishingEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FishingTechniques",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    InstructorHasBoat = table.Column<bool>(nullable: false),
                    Inshore = table.Column<bool>(nullable: false),
                    Offshore = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishingTechniques", x => x.Id);
                });

           /* migrationBuilder.CreateTable(
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
                });*/

            migrationBuilder.CreateTable(
                name: "InstructorReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    InstructorId = table.Column<string>(nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivilegedUserRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegedUserRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDeleteRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserDetailsId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeleteRequest", x => x.Id);
                });

            /*migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IdentityUserId = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    PenaltyCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                });*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adventure");

            migrationBuilder.DropTable(
                name: "AdventureFavorites");

            migrationBuilder.DropTable(
                name: "AdventureFishingEquipment");

            migrationBuilder.DropTable(
                name: "AdventureFishingTechniques");

            migrationBuilder.DropTable(
                name: "AdventureRealisation");

            migrationBuilder.DropTable(
                name: "AdventureReservation");

            migrationBuilder.DropTable(
                name: "AdventureReservationReview");

            migrationBuilder.DropTable(
                name: "AdventureReview");

            migrationBuilder.DropTable(
                name: "AdventureRules");

            migrationBuilder.DropTable(
                name: "AdventuresAdventureRules");

            migrationBuilder.DropTable(
                name: "AdventureSpecialOffer");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Boat");

            migrationBuilder.DropTable(
                name: "BoatAmenities");

            migrationBuilder.DropTable(
                name: "BoatFavorites");

            migrationBuilder.DropTable(
                name: "BoatFishingEquipment");

           /* migrationBuilder.DropTable(
                name: "BoatOwner");*/

            migrationBuilder.DropTable(
                name: "BoatOwnerReview");

            migrationBuilder.DropTable(
                name: "BoatReservation");

            migrationBuilder.DropTable(
                name: "BoatReservationReview");

            migrationBuilder.DropTable(
                name: "BoatReview");

            migrationBuilder.DropTable(
                name: "BoatRules");

            migrationBuilder.DropTable(
                name: "BoatsBoatRules");

            migrationBuilder.DropTable(
                name: "BoatSpecialOffer");

           /* migrationBuilder.DropTable(
                name: "CottageOwner");*/

            migrationBuilder.DropTable(
                name: "FishingEquipment");

            migrationBuilder.DropTable(
                name: "FishingTechniques");

           /* migrationBuilder.DropTable(
                name: "Instructor");*/

            migrationBuilder.DropTable(
                name: "InstructorReview");

            migrationBuilder.DropTable(
                name: "PrivilegedUserRequest");

            migrationBuilder.DropTable(
                name: "UserDeleteRequest");

            /*migrationBuilder.DropTable(
                name: "UserDetails");*/

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CottageSpecialOffer");

            migrationBuilder.DropColumn(
                name: "ReceivedPenalty",
                table: "CottageReservationReview");

            migrationBuilder.DropColumn(
                name: "MaxPersonCount",
                table: "CottageReservation");

            migrationBuilder.DropColumn(
                name: "RegularPrice",
                table: "Cottage");

            migrationBuilder.DropColumn(
                name: "WeekendPrice",
                table: "Cottage");

            migrationBuilder.AddColumn<int>(
                name: "PersonCount",
                table: "CottageReservation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
