using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Pilots;

public class DetailsModel(RazoAirDbContext db) : PageModel
{
    public Pilot Pilot { get; set; } = null!;
    public List<Flight> UpcomingFlights { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var pilot = await db.Pilots
            .AsNoTracking()
            .Include(p => p.Airline)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pilot is null)
        {
            return NotFound();
        }

        Pilot = pilot;

        var today = DateTime.Today;
        UpcomingFlights = await db.Flights
            .AsNoTracking()
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Where(f => (f.CaptainId == id || f.FirstOfficerId == id) && f.DepartureTime >= today)
            .OrderBy(f => f.DepartureTime)
            .Take(6)
            .ToListAsync();

        return Page();
    }
}
