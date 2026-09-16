using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazoAir.Web.Models;

/// <summary>A booked ticket: one passenger on one seat of one flight.</summary>
public class Ticket
{
    public int Id { get; set; }

    [Required, StringLength(8)]
    public string BookingReference { get; set; } = string.Empty;

    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;

    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;

    [Required, StringLength(100)]
    public string PassengerFullName { get; set; } = string.Empty;

    [Required, StringLength(150), EmailAddress]
    public string PassengerEmail { get; set; } = string.Empty;

    [StringLength(30)]
    public string? PassengerPhone { get; set; }

    [Required, StringLength(20)]
    public string PassengerDocumentNumber { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(10,2)")]
    public decimal PricePaid { get; set; }
}
