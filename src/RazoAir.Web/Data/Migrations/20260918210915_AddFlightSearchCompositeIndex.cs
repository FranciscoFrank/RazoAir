using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazoAir.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightSearchCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PassengerEmail",
                table: "Tickets",
                column: "PassengerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureAirportId_ArrivalAirportId_DepartureTime",
                table: "Flights",
                columns: new[] { "DepartureAirportId", "ArrivalAirportId", "DepartureTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_PassengerEmail",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DepartureAirportId_ArrivalAirportId_DepartureTime",
                table: "Flights");
        }
    }
}
