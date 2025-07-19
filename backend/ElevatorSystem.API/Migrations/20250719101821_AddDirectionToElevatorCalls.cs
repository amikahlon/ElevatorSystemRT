using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevatorSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectionToElevatorCalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "ElevatorCalls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "ElevatorCalls");
        }
    }
}
