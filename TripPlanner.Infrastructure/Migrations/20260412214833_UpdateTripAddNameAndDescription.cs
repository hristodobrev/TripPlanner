using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripAddNameAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Trips",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Trips");
        }
    }
}
