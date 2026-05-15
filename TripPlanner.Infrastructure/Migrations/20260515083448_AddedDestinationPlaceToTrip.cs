using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDestinationPlaceToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_PlaceDetails_PlaceDetailsId",
                table: "TripPlaces");

            migrationBuilder.RenameColumn(
                name: "PlaceDetailsId",
                table: "TripPlaces",
                newName: "PlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Places_PlaceDetailsId",
                table: "TripPlaces",
                newName: "IX_TripPlaces_PlaceId");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationPlaceId",
                table: "Trips",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO Places (Id, ExternalId, Name, CreatedAtUtc)
                SELECT
                    NEWID(),
                    t.DestinationExternalId,
                    MAX(t.DestinationName),
                    GETUTCDATE()
                FROM Trips t
                WHERE t.DestinationExternalId IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Places p
                      WHERE p.ExternalId = t.DestinationExternalId
                  )
                GROUP BY t.DestinationExternalId;
            """);

            migrationBuilder.Sql("""
                UPDATE t
                SET DestinationPlaceId = p.Id
                FROM Trips t
                INNER JOIN Places p 
                    ON p.ExternalId = t.DestinationExternalId
                WHERE t.DestinationPlaceId IS NULL;
            """);

            migrationBuilder.AlterColumn<Guid>(
                name: "DestinationPlaceId",
                table: "Trips",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DestinationPlaceId",
                table: "Trips",
                column: "DestinationPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripPlaces_Places_PlaceId",
                table: "TripPlaces",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Places_DestinationPlaceId",
                table: "Trips",
                column: "DestinationPlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripPlaces_Places_PlaceId",
                table: "TripPlaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Places_DestinationPlaceId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DestinationPlaceId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DestinationPlaceId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "TripPlaces",
                newName: "PlaceDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_TripPlaces_PlaceId",
                table: "TripPlaces",
                newName: "IX_TripPlaces_PlaceDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripPlaces_Places_PlaceDetailsId",
                table: "TripPlaces",
                column: "PlaceDetailsId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
