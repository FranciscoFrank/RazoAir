using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Aircraft
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Model { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>Number of seat rows, e.g. 30.</summary>
    public int Rows { get; set; }

    /// <summary>Seat letters per row in order, e.g. "ABC DEF" (space marks the aisle).</summary>
    [Required, StringLength(12)]
    public string SeatLayout { get; set; } = "ABC DEF";

    /// <summary>How many of the leading rows are Business class.</summary>
    public int BusinessRows { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public int TotalSeats => Seats.Count;
}
