using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ma.Web.Data.Migrations
{
    public partial class AddAppointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    IsBirthday = table.Column<bool>(nullable: false),
                    Repetitions = table.Column<byte>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Date = table.Column<DateTimeOffset>(nullable: true),
                    RemindeBeforeDays = table.Column<int>(nullable: true),
                    RemindeBeforeWeeks = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
