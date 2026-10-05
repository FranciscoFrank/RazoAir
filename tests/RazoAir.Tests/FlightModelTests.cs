using RazoAir.Web.Models;

namespace RazoAir.Tests;

public class FlightModelTests
{
    [Fact]
    public void Duration_CalculatesDifferenceBetweenArrivalAndDeparture()
    {
        var departure = new DateTime(2026, 10, 1, 10, 0, 0);
        var arrival = new DateTime(2026, 10, 1, 12, 45, 0);

        var flight = new Flight
        {
            DepartureTime = departure,
            ArrivalTime = arrival,
        };

        Assert.Equal(TimeSpan.FromMinutes(165), flight.Duration);
    }

    [Fact]
    public void Aircraft_SeatLayout_CanBeParsedIntoAisleAndColumns()
    {
        var aircraft = new Aircraft
        {
            Model = "A320neo",
            SeatLayout = "ABC DEF",
        };

        var aisleIndex = aircraft.SeatLayout.IndexOf(' ');
        var columns = aircraft.SeatLayout.Where(c => c != ' ').ToArray();

        Assert.Equal(3, aisleIndex);
        Assert.Equal(['A', 'B', 'C', 'D', 'E', 'F'], columns);
    }
}
