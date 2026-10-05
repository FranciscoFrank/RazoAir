using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Account;

[EnableRateLimiting("sensitive")]
public class LoginModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Enter your email")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        if (result.IsLockedOut)
        {
            ErrorMessage = "This account is temporarily locked due to repeated failed attempts. Please try again in 5 minutes.";
            return Page();
        }

        ErrorMessage = "Incorrect email or password.";
        return Page();
    }
}
