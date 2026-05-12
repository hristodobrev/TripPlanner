using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlaceDetailsFixedTypoInPlace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationMinues",
                table: "Places",
                newName: "DurationMinutes");

            migrationBuilder.CreateTable(
                name: "PlaceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceDetails", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceDetailsId",
                table: "Places",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO PlaceDetails (Id, ExternalId, Name, Country, Description, ImageUrl, CreatedAtUtc)
                SELECT
                    NEWID(),
                    p.ExternalId,
                    MAX(p.Name),
                    NULL,
                    NULL,
                    NULL,
                    GETUTCDATE()
                FROM Places p
                WHERE p.ExternalId IS NOT NULL
                GROUP BY p.ExternalId;
            """);

            migrationBuilder.Sql("""
                UPDATE p
                SET p.PlaceDetailsId = pd.Id
                FROM Places p
                INNER JOIN PlaceDetails pd ON pd.ExternalId = p.ExternalId;
            """);

            migrationBuilder.AlterColumn<Guid>(
                name: "PlaceDetailsId",
                table: "Places",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Places");

            migrationBuilder.CreateIndex(
                name: "IX_Places_PlaceDetailsId",
                table: "Places",
                column: "PlaceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceDetails_ExternalId",
                table: "PlaceDetails",
                column: "ExternalId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_PlaceDetails_PlaceDetailsId",
                table: "Places",
                column: "PlaceDetailsId",
                principalTable: "PlaceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_PlaceDetails_PlaceDetailsId",
                table: "Places");

            migrationBuilder.DropTable(
                name: "PlaceDetails");

            migrationBuilder.DropIndex(
                name: "IX_Places_PlaceDetailsId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PlaceDetailsId",
                table: "Places");

            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Places",
                newName: "DurationMinues");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Places",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
