using Microsoft.EntityFrameworkCore.Migrations;

namespace EventTracker.DAL.Migrations
{
    public partial class FixedLocationOneToOneRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Location_LocationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_LocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Location",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Location_EventId",
                table: "Location",
                column: "EventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Events_EventId",
                table: "Location",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Events_EventId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_EventId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Location");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Location_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
