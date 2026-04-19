using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredTripEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Places_DestinationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DestinationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "ExternalPlaceId",
                table: "Places",
                newName: "ExternalId");

            migrationBuilder.AddColumn<string>(
                name: "DestinationExternalId",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationName",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationExternalId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DestinationName",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Places",
                newName: "ExternalPlaceId");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationId",
                table: "Trips",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DestinationId",
                table: "Trips",
                column: "DestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Places_DestinationId",
                table: "Trips",
                column: "DestinationId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
