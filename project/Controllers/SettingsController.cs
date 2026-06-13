using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Services;

namespace project.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserSettingsService _settingsService;

    public SettingsController(UserManager<ApplicationUser> userManager, UserSettingsService settingsService)
    {
        _userManager = userManager;
        _settingsService = settingsService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();
        var settings = await _settingsService.GetAsync(user);
        var model = new SettingsViewModel { User = user, Settings = settings };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfilePublic(bool isPublic)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();
        await _settingsService.UpdateAsync(user, s => s.ProfilePublic = isPublic);
        return Ok(new { success = true });
    }
}
