using Microsoft.EntityFrameworkCore.Migrations;

namespace Ma.Services.Appointments.Migrations
{
    public partial class RemovedIsBirthday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBirthday",
                table: "Appointments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBirthday",
                table: "Appointments",
                nullable: false,
                defaultValue: false);
        }
    }
}
