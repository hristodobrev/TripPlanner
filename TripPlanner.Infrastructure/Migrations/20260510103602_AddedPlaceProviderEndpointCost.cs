using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlaceProviderEndpointCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceProviderEndpointCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceProvider = table.Column<int>(type: "int", nullable: false),
                    EndpointType = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceProviderEndpointCosts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PlaceProviderEndpointCosts",
                columns: new[] { "Id", "Cost", "CreatedAtUtc", "EndpointType", "PlaceProvider" },
                values: new object[,]
                {
                    { new Guid("24b519c7-3cd5-4e03-984f-97e4946e1b5d"), 0.00283m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 1, 1 },
                    { new Guid("50511583-e3fd-4997-87c7-745cd25118d2"), 0.0m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 2, 1 },
                    { new Guid("6d6656ed-616e-48c9-b990-f69fa10e58d6"), 0.005m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 3, 1 },
                    { new Guid("734eed77-4170-440b-87a5-49cd0eba1232"), 0.017m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 4, 1 },
                    { new Guid("80be5d69-4b9a-4325-bc25-98d5a4d1339b"), 0.02m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 5, 1 },
                    { new Guid("90763803-9b7c-43ea-b2e6-2c1eec863f23"), 0.007m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 6, 1 },
                    { new Guid("9c05f5f2-ad9f-4c23-9a5a-32d84d5bf7d6"), 0.0m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 7, 1 },
                    { new Guid("9ff32d8d-ba07-4cff-abbc-b17a25449cb7"), 0.032m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 8, 1 },
                    { new Guid("a399866c-d277-46cd-9808-0e84dd0589e5"), 0.035m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 9, 1 },
                    { new Guid("b20f5d08-fff1-4a64-87d5-82ed2f54628b"), 0.032m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 10, 1 },
                    { new Guid("c777bb6d-9f16-473b-8522-d7426b7d30dd"), 0.035m, new DateTime(2026, 5, 10, 10, 29, 50, 0, DateTimeKind.Utc), 11, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceProviderEndpointCosts");
        }
    }
}
