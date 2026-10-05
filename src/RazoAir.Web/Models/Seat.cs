using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Seat
{
    public int Id { get; set; }

    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;

    public int Row { get; set; }

    [Required, StringLength(1)]
    public string Letter { get; set; } = "A";

    public SeatClass SeatClass { get; set; } = SeatClass.Economy;

    public bool IsWindow { get; set; }
    public bool IsAisle { get; set; }

    public string Number => $"{Row}{Letter}";

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
