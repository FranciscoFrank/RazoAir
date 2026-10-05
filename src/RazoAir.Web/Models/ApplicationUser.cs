using Microsoft.AspNetCore.Identity;

namespace RazoAir.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string? AvatarFileName { get; set; }
}
