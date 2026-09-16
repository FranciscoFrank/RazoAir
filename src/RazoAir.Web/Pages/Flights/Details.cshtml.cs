using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Flights;

public class DetailsModel(RazoAirDbContext db) : PageModel
{
    public Flight Flight { get; set; } = null!;
    public List<Seat> Seats { get; set; } = [];
    public HashSet<int> BookedSeatIds { get; set; } = [];
    public char[] Columns { get; set; } = [];
    public int AisleIndex { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var flight = await db.Flights
            .Include(f => f.Airline)
            .Include(f => f.Aircraft)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Include(f => f.Captain)
            .Include(f => f.FirstOfficer)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight is null)
        {
            return NotFound();
        }

        Flight = flight;

        Seats = await db.Seats
            .Where(s => s.AircraftId == flight.AircraftId)
            .OrderBy(s => s.Row).ThenBy(s => s.Letter)
            .ToListAsync();

        BookedSeatIds = (await db.Tickets
                .Where(t => t.FlightId == id)
                .Select(t => t.SeatId)
                .ToListAsync())
            .ToHashSet();

        AisleIndex = flight.Aircraft.SeatLayout.IndexOf(' ');
        Columns = flight.Aircraft.SeatLayout.Where(c => c != ' ').ToArray();

        return Page();
    }
}
