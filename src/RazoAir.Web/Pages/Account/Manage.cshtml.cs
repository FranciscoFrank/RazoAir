using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazoAir.Web.Models;
using RazoAir.Web.Security;

namespace RazoAir.Web.Pages.Account;

[Authorize]
public class ManageModel(UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : PageModel
{
    private static readonly Dictionary<string, string> AllowedAvatarTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = ".png",
        ["image/jpeg"] = ".jpg",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif",
    };

    private const long MaxAvatarBytes = 2 * 1024 * 1024;

    [BindProperty]
    public string? PhoneNumber { get; set; }

    [BindProperty]
    public IFormFile? Avatar { get; set; }

    public string? Email { get; set; }
    public string? AvatarFileName { get; set; }
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        AvatarFileName = user.AvatarFileName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        Email = user.Email;
        AvatarFileName = user.AvatarFileName;

        if (Avatar is not null)
        {
            if (Avatar.Length > MaxAvatarBytes)
            {
                ErrorMessage = "Avatar must be smaller than 2 MB.";
                return Page();
            }

            if (!AllowedAvatarTypes.TryGetValue(Avatar.ContentType, out var extension))
            {
                ErrorMessage = "Avatar must be a PNG, JPEG, WEBP or GIF image.";
                return Page();
            }

            await using (var avatarStream = Avatar.OpenReadStream())
            {
                if (!ImageValidator.IsValidImageSignature(avatarStream, extension))
                {
                    ErrorMessage = "Avatar file content is corrupted or invalid.";
                    return Page();
                }
            }

            var avatarsDir = Path.Combine(env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(avatarsDir);

            var fileName = $"{user.Id}{extension}";
            var filePath = Path.Combine(avatarsDir, fileName);

            if (user.AvatarFileName is not null && user.AvatarFileName != fileName)
            {
                var oldPath = Path.Combine(avatarsDir, user.AvatarFileName);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            await using (var stream = System.IO.File.Create(filePath))
            {
                await Avatar.CopyToAsync(stream);
            }

            user.AvatarFileName = fileName;
            AvatarFileName = fileName;
        }

        user.PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim();

        await userManager.UpdateAsync(user);

        PhoneNumber = user.PhoneNumber;
        StatusMessage = "Your account has been updated.";
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAvatarAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        if (user.AvatarFileName is not null)
        {
            var path = Path.Combine(env.WebRootPath, "uploads", "avatars", user.AvatarFileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            user.AvatarFileName = null;
            await userManager.UpdateAsync(user);
        }

        return RedirectToPage();
    }
}
