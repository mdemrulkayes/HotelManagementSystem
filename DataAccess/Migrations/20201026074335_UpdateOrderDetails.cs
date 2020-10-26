using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "RoomOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RoomOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "RoomOrderDetails");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RoomOrderDetails");
        }
    }
}
