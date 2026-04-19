using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredPlaceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Places_PlaceId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "TripPlaces");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "Trips",
                newName: "DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_PlaceId",
                table: "Trips",
                newName: "IX_Trips_DestinationId");

            migrationBuilder.AddColumn<Guid>(
                name: "TripId",
                table: "Places",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Places_TripId",
                table: "Places",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Trips_TripId",
                table: "Places",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Places_DestinationId",
                table: "Trips",
                column: "DestinationId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Trips_TripId",
                table: "Places");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Places_DestinationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Places_TripId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "Places");

            migrationBuilder.RenameColumn(
                name: "DestinationId",
                table: "Trips",
                newName: "PlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_DestinationId",
                table: "Trips",
                newName: "IX_Trips_PlaceId");

            migrationBuilder.CreateTable(
                name: "TripPlaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripPlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripPlaces_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripPlaces_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripPlaces_PlaceId",
                table: "TripPlaces",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TripPlaces_TripId_PlaceId",
                table: "TripPlaces",
                columns: new[] { "TripId", "PlaceId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Places_PlaceId",
                table: "Trips",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
