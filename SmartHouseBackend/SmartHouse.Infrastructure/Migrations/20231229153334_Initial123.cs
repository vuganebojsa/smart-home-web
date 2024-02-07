using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveDays",
                table: "SmartDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndSprinkle",
                table: "SmartDevices",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialMode",
                table: "SmartDevices",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartSprinkle",
                table: "SmartDevices",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveDays",
                table: "SmartDevices");

            migrationBuilder.DropColumn(
                name: "EndSprinkle",
                table: "SmartDevices");

            migrationBuilder.DropColumn(
                name: "IsSpecialMode",
                table: "SmartDevices");

            migrationBuilder.DropColumn(
                name: "StartSprinkle",
                table: "SmartDevices");
        }
    }
}
