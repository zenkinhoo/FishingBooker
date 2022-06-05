using Microsoft.EntityFrameworkCore.Migrations;

namespace Hooking.Data.Migrations
{
    public partial class adventurefieldcancellationtocancellationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelationPolicy",
                table: "Adventure");

            migrationBuilder.AddColumn<string>(
                name: "CancellationPolicyId",
                table: "Adventure",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationPolicyId",
                table: "Adventure");

            migrationBuilder.AddColumn<string>(
                name: "CancelationPolicy",
                table: "Adventure",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
