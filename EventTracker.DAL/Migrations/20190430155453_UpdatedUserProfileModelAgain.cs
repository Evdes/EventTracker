using Microsoft.EntityFrameworkCore.Migrations;

namespace EventTracker.DAL.Migrations
{
    public partial class UpdatedUserProfileModelAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeFrame_Events_EventId",
                table: "TimeFrame");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeFrame",
                table: "TimeFrame");

            migrationBuilder.DropColumn(
                name: "LoginEmail",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "TimeFrame",
                newName: "Timeframe");

            migrationBuilder.RenameIndex(
                name: "IX_TimeFrame_EventId",
                table: "Timeframe",
                newName: "IX_Timeframe_EventId");

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmPassword",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Timeframe",
                table: "Timeframe",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeframe_Events_EventId",
                table: "Timeframe",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timeframe_Events_EventId",
                table: "Timeframe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Timeframe",
                table: "Timeframe");

            migrationBuilder.RenameTable(
                name: "Timeframe",
                newName: "TimeFrame");

            migrationBuilder.RenameIndex(
                name: "IX_Timeframe_EventId",
                table: "TimeFrame",
                newName: "IX_TimeFrame_EventId");

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmPassword",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginEmail",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeFrame",
                table: "TimeFrame",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeFrame_Events_EventId",
                table: "TimeFrame",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
