using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Booking;

public class CreateModel(RazoAirDbContext db, UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int FlightId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SeatId { get; set; }

    [BindProperty]
    public PassengerInput Passenger { get; set; } = new();

    public Flight Flight { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
    public decimal Price => Seat.SeatClass == SeatClass.Business ? Flight.BusinessPrice : Flight.EconomyPrice;

    public string? ErrorMessage { get; set; }

    public class PassengerInput
    {
        [Required(ErrorMessage = "Enter your full name"), StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your email"), EmailAddress(ErrorMessage = "Enter a valid email"), StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Enter your document number"), StringLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var loaded = await LoadFlightAndSeatAsync();
        if (loaded is not null)
        {
            return loaded;
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser is not null)
            {
                Passenger.Email = currentUser.Email ?? string.Empty;
                Passenger.Phone = currentUser.PhoneNumber;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var loaded = await LoadFlightAndSeatAsync();
        if (loaded is not null)
        {
            return loaded;
        }

        var alreadyBooked = await db.Tickets.AnyAsync(t => t.FlightId == FlightId && t.SeatId == SeatId);
        if (alreadyBooked)
        {
            ErrorMessage = "Sorry, that seat was just booked. Please choose another one.";
            ModelState.Clear();
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var ticket = new Ticket
        {
            BookingReference = await GenerateUniqueReferenceAsync(),
            FlightId = FlightId,
            SeatId = SeatId,
            PassengerFullName = Passenger.FullName.Trim(),
            PassengerEmail = Passenger.Email.Trim(),
            PassengerPhone = Passenger.Phone?.Trim(),
            PassengerDocumentNumber = Passenger.DocumentNumber.Trim(),
            PricePaid = Price,
            CreatedAtUtc = DateTime.UtcNow,
        };

        db.Tickets.Add(ticket);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ErrorMessage = "Sorry, that seat was just booked. Please choose another one.";
            return Page();
        }

        return RedirectToPage("/Booking/Confirmation", new { reference = ticket.BookingReference });
    }

    private async Task<IActionResult?> LoadFlightAndSeatAsync()
    {
        var flight = await db.Flights
            .Include(f => f.Airline)
            .Include(f => f.Aircraft)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .FirstOrDefaultAsync(f => f.Id == FlightId);

        var seat = await db.Seats.FirstOrDefaultAsync(s => s.Id == SeatId);

        if (flight is null || seat is null || seat.AircraftId != flight.AircraftId)
        {
            return NotFound();
        }

        Flight = flight;
        Seat = seat;
        return null;
    }

    private async Task<string> GenerateUniqueReferenceAsync()
    {
        string reference;
        do
        {
            reference = BookingReferenceGenerator.Generate();
        } while (await db.Tickets.AnyAsync(t => t.BookingReference == reference));

        return reference;
    }
}
