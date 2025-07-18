using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevatorSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialElevatorCallTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElevatorCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    ElevatorId = table.Column<int>(type: "int", nullable: true),
                    RequestedFloor = table.Column<int>(type: "int", nullable: false),
                    DestinationFloor = table.Column<int>(type: "int", nullable: true),
                    CallTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHandled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElevatorCalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElevatorCalls_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElevatorCalls_Elevators_ElevatorId",
                        column: x => x.ElevatorId,
                        principalTable: "Elevators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ElevatorCallAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElevatorCallId = table.Column<int>(type: "int", nullable: false),
                    ElevatorId = table.Column<int>(type: "int", nullable: false),
                    AssignmentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElevatorCallAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElevatorCallAssignments_ElevatorCalls_ElevatorCallId",
                        column: x => x.ElevatorCallId,
                        principalTable: "ElevatorCalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElevatorCallAssignments_Elevators_ElevatorId",
                        column: x => x.ElevatorId,
                        principalTable: "Elevators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElevatorCallAssignments_ElevatorCallId",
                table: "ElevatorCallAssignments",
                column: "ElevatorCallId");

            migrationBuilder.CreateIndex(
                name: "IX_ElevatorCallAssignments_ElevatorId",
                table: "ElevatorCallAssignments",
                column: "ElevatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ElevatorCalls_BuildingId",
                table: "ElevatorCalls",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ElevatorCalls_ElevatorId",
                table: "ElevatorCalls",
                column: "ElevatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElevatorCallAssignments");

            migrationBuilder.DropTable(
                name: "ElevatorCalls");
        }
    }
}
