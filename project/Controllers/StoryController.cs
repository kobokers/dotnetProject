using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models;

namespace project.Controllers;

[Authorize]
public class StoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ModelState.AddModelError("", "Please select an image.");
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "stories", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var story = new Story
        {
            UserId = user!.Id,
            ImageUrl = $"/uploads/stories/{fileName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Post");
    }
}
