using Microsoft.AspNetCore.Identity;

namespace RazoAir.Web.Models;

/// <summary>
/// Registration only collects email + password; phone number (built into IdentityUser)
/// and the avatar below are added later from the account page.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? AvatarFileName { get; set; }
}
