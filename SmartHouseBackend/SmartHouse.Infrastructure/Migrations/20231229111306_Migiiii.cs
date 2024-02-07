using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migiiii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PercentageOfCharge",
                table: "SmartDevices",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentageOfCharge",
                table: "SmartDevices");
        }
    }
}
