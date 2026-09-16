using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;

namespace RazoAir.Web.Pages.Booking;

public class LookupModel(RazoAirDbContext db) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Enter your booking reference")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Booking references are 6 characters long")]
    public string? Reference { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Enter your email")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string? Email { get; set; }

    public string? ErrorMessage { get; set; }
    public bool Submitted { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Submitted = true;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var reference = Reference!.Trim().ToUpperInvariant();
        var email = Email!.Trim();

        var exists = await db.Tickets.AnyAsync(t =>
            t.BookingReference == reference &&
            t.PassengerEmail.ToLower() == email.ToLower());

        if (!exists)
        {
            ErrorMessage = "Booking not found. Check your reference and email.";
            return Page();
        }

        return RedirectToPage("/Booking/Confirmation", new { reference });
    }
}
