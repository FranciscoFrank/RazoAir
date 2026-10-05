using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Pilot
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public PilotRank Rank { get; set; } = PilotRank.Captain;

    public int AirlineId { get; set; }
    public Airline Airline { get; set; } = null!;

    public int FlightHours { get; set; }

    public int YearsOfExperience { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(300)]
    public string? Certifications { get; set; }

    public ICollection<Flight> FlightsAsCaptain { get; set; } = new List<Flight>();
    public ICollection<Flight> FlightsAsFirstOfficer { get; set; } = new List<Flight>();

    public string RankLabel => Rank switch
    {
        PilotRank.SeniorCaptain => "Senior Captain",
        PilotRank.Captain => "Captain",
        _ => "First Officer"
    };

    public string Initials => string.Join("", FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Take(2)
        .Select(p => char.ToUpperInvariant(p[0])));
}
