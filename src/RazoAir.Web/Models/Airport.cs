using System.ComponentModel.DataAnnotations;

namespace RazoAir.Web.Models;

public class Airport
{
    public int Id { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string IataCode { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Country { get; set; } = string.Empty;

    public ICollection<Flight> DeparturesFrom { get; set; } = new List<Flight>();
    public ICollection<Flight> ArrivalsTo { get; set; } = new List<Flight>();

    public string DisplayName => $"{City} ({IataCode})";
}
