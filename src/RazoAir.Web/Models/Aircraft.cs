using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Aircraft
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Model { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Manufacturer { get; set; } = string.Empty;

    public int Rows { get; set; }

    [Required, StringLength(12)]
    public string SeatLayout { get; set; } = "ABC DEF";

    public int BusinessRows { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public int TotalSeats => Seats.Count;
}
