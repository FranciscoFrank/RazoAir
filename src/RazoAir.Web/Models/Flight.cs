using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazoAir.Web.Models;

public class Flight
{
    public int Id { get; set; }

    [Required, StringLength(8)]
    public string FlightNumber { get; set; } = string.Empty;

    public int AirlineId { get; set; }
    public Airline Airline { get; set; } = null!;

    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;

    public int DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;

    public int ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;

    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }

    public int CaptainId { get; set; }
    public Pilot Captain { get; set; } = null!;

    public int FirstOfficerId { get; set; }
    public Pilot FirstOfficer { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal EconomyPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal BusinessPrice { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    [NotMapped]
    public TimeSpan Duration => ArrivalTime - DepartureTime;
}
