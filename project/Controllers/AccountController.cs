using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email and password are required.");
            return View();
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? email.Split('@')[0],
            FriendCode = email.Split('@')[0] + "#" + new Random().Next(1000, 9999).ToString(),
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email and password are required.");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && string.IsNullOrEmpty(user.FriendCode))
            {
                user.FriendCode = user.UserName + "#" + new Random().Next(1000, 9999).ToString();
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(string? id)
    {
        if (id == null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(string displayName, string? bio, string? location, IFormFile? profilePhoto, IFormFile? coverPhotoFile)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.DisplayName = displayName;
        user.Bio = bio;
        user.Location = location;

        if (profilePhoto != null && profilePhoto.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(profilePhoto.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePhoto.CopyToAsync(stream);
            }

            user.ProfilePhoto = $"/uploads/profiles/{fileName}";
        }

        if (coverPhotoFile != null && coverPhotoFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(coverPhotoFile.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverPhotoFile.CopyToAsync(stream);
            }

            user.CoverPhoto = fileName;
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Profile");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var users = string.IsNullOrWhiteSpace(query)
            ? _userManager.Users.Take(0)
            : _userManager.Users.Where(u =>
                u.DisplayName!.Contains(query) || u.UserName!.Contains(query) || u.FriendCode.Contains(query));

        ViewBag.Query = query;
        return View(await users.Take(20).ToListAsync());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        await _signInManager.SignOutAsync();
        await _userManager.DeleteAsync(user);
        return RedirectToAction("Index", "Home");
    }
}
