using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migigigi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RoomHumidity",
                table: "SmartDevices",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoomTemperature",
                table: "SmartDevices",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomHumidity",
                table: "SmartDevices");

            migrationBuilder.DropColumn(
                name: "RoomTemperature",
                table: "SmartDevices");
        }
    }
}
