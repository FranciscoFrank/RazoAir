using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Airline
{
    public int Id { get; set; }

    [Required, StringLength(2, MinimumLength = 2)]
    public string IataCode { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    [StringLength(7)]
    public string AccentColor { get; set; } = "#5B4B7A";

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    public ICollection<Pilot> Pilots { get; set; } = new List<Pilot>();
}
