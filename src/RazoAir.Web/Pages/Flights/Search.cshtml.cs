using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Flights;

public class SearchModel(RazoAirDbContext db) : PageModel
{
    public class FlightGroup
    {
        public Airport From { get; set; } = null!;
        public Airport To { get; set; } = null!;
        public DateTime EarliestDeparture { get; set; }
        public DateTime LatestDeparture { get; set; }
        public List<Flight> Options { get; set; } = [];
    }

    public string? From { get; set; }
    public string? To { get; set; }
    public string? AirlineFilter { get; set; }
    public DateTime Date { get; set; }

    public List<Airport> Airports { get; set; } = [];
    public List<Airline> Airlines { get; set; } = [];
    public List<FlightGroup> Groups { get; set; } = [];
    public bool HasSearched { get; set; }

    public async Task OnGetAsync(string? from, string? to, DateTime? date, string? airline)
    {
        Airports = await db.Airports.OrderBy(a => a.City).ToListAsync();
        Airlines = await db.Airlines.OrderBy(a => a.Name).ToListAsync();

        From = from;
        To = to;
        AirlineFilter = airline;
        Date = date?.Date ?? DateTime.Today.AddDays(1);

        if (string.IsNullOrWhiteSpace(from))
        {
            return;
        }

        HasSearched = true;

        var query = db.Flights
            .Include(f => f.Airline)
            .Include(f => f.Aircraft)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Where(f => f.DepartureAirport.IataCode == from && f.DepartureTime.Date == Date.Date);

        if (!string.IsNullOrWhiteSpace(to))
        {
            query = query.Where(f => f.ArrivalAirport.IataCode == to);
        }

        if (!string.IsNullOrWhiteSpace(airline))
        {
            query = query.Where(f => f.Airline.IataCode == airline);
        }

        var flights = await query.OrderBy(f => f.DepartureTime).ToListAsync();

        // Group flights bound for the same destination that depart within the same hour,
        // so travellers see them as directly comparable options for that date/time/airport.
        Groups = flights
            .GroupBy(f => (f.ArrivalAirportId, f.DepartureTime.Date, f.DepartureTime.Hour))
            .Select(g => new FlightGroup
            {
                From = g.First().DepartureAirport,
                To = g.First().ArrivalAirport,
                EarliestDeparture = g.Min(f => f.DepartureTime),
                LatestDeparture = g.Max(f => f.DepartureTime),
                Options = g.OrderBy(f => f.DepartureTime).ToList(),
            })
            .OrderBy(g => g.EarliestDeparture)
            .ToList();
    }
}
